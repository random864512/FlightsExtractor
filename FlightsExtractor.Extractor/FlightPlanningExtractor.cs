using FlightsExtractor.Extractor.Factories;

namespace FlightsExtractor.Extractor;

/*
    public interface representing pdf extractor
    depending on needs (like passing lib to 3rd parties, public nuget etc.) to completely avoid eventual changes due to implementation details it could look like:
    public interface IFlightPlanningExtractor : IDisposable, IAsyncDisposable
    {
        Task<Result<FlightPlanning>> Extract(string file, CancellationToken token);
    }
    to avoid breaking changes in case of library switch or update in the future
 */

public interface IFlightPlanningExtractor
{
    /// <exception cref="FileDoesNotExistException">File does not exist</exception>
    /// <exception cref="FlightPlanningExtractionException">File cannot be parsed due to internal error or invalid file format</exception>
    Result<FlightPlanning> Extract(string file);
}

internal class FlightPlanningExtractor(
    FlightNumberFactory flightNumberFactory,
    AircraftRegistrationFactory aircraftRegistrationFactory,
    ICAOAAirportCodeFactory airportCodeFactory,
    Messages messages,
    ILogger<FlightPlanningExtractor>? logger = default)
    : IFlightPlanningExtractor
{
    public Result<FlightPlanning> Extract(string file)
    {
        logger?.LogDebug("Checking if file exists");
        if (!File.Exists(file))
            throw new FileDoesNotExistException();

        try
        {
            logger?.LogDebug("Opening file");
            using var document = FlightPlanningDocument.Create(file);

            FlightPlanning flightPlanning = new(Flights: []);
            foreach (var planning in document.PlanningPages)
            {
                var flightNumber = planning.ParseFlightNumber().ToResult(error: messages.FlightNumberIsNotAvailable).MapResult(flightNumberFactory.Create);
                if (!flightNumber.IsSuccess)
                    return Error<FlightPlanning>(flightNumber.Error!).OnError(LogWarn);

                var flightDate = planning.ParseFlightDate().ToResult(error: messages.FlightDateIsNotAvailable);
                if (!flightDate.IsSuccess)
                    return Error<FlightPlanning>(flightNumber.Error!).OnError(LogWarn);

                var briefing = document.BriefingPages.FirstOrDefault(x => flightNumber.Value!.Number.Equals(x.ParseCrewTable()?.flightNumber)).ToResult(messages.BriefingPageIsMissing);
                if (!briefing.IsSuccess)
                    return Error<FlightPlanning>(briefing.Error!).OnError(LogWarn);

                flightPlanning = flightPlanning with
                {
                    Flights = [ .. flightPlanning.Flights,
                    new Flight(
                        flightNumber!,
                        flightDate,
                        planning.ParseAircraftRegistration().ToResult(messages.AircraftRegistrationIsNotAvailable).MapResult(aircraftRegistrationFactory.Create).OnError(LogWarn),
                        new Route(
                            planning.ParseFrom().ToResult(messages.FromIsNotAvailable).MapResult(airportCodeFactory.Create).OnError(LogWarn),
                            planning.ParseTo().ToResult(messages.ToIsNotAvailable).MapResult(airportCodeFactory.Create).OnError(LogWarn)
                        ),
                        planning.ParseAlternativeAirdrom1().ToResult(messages.Airdrom1IsNotAvailable).MapResult(airportCodeFactory.Create).OnError(LogWarn),
                        planning.ParseAlternativeAirdrom2().ToResult(messages.Airdrom2IsNotAvailable).MapResult(airportCodeFactory.Create).OnError(LogWarn),
                        planning.ParseATCCallSign().ToResult(messages.ATCCallSignIsNotAvailable).OnError(LogWarn),
                        (planning.ParseTo().MapNullable(planning.ParseFuelToAirport)?.Time).ToResult(messages.TimeToDestinationIsNoTAvailable).OnError(LogWarn),
                        (planning.ParseTo().MapNullable(planning.ParseFuelToAirport)?.Fuel).ToResult(messages.FuelToDestinationIsNotAvailable).OnError(LogWarn),
                        (planning.ParseAlternativeAirdrom1().MapNullable(planning.ParseFuelToAirport)?.Time).ToResult(messages.TimeToAlternativeIsNotAvailable).OnError(LogWarn),
                        (planning.ParseAlternativeAirdrom1().MapNullable(planning.ParseFuelToAirport)?.Fuel).ToResult(messages.FuelMinIsNotAvailable).OnError(LogWarn),
                        planning.ParseFuelMin().ToResult(messages.FuelMinIsNotAvailable).OnError(LogWarn),
                        (briefing.Value!.ParseCrewTable()?.crewMembers?.Select(parsed =>
                            new CrewMember(parsed.function.ToResult(messages.CrewMemberFunctionIsNotAvailable).OnError(LogWarn),
                            parsed.name.ToResult(messages.CrewMembersNameIsNotAvailable).OnError(LogWarn))).ToImmutableList()).ToResult(messages.CrewMembersListIsNotAvailable).OnError(LogWarn)
                    )
                    ]
                };
            }

            if (flightPlanning.Flights.Count != document.PlanningPages.Count ||
                flightPlanning.Flights.Count != document.BriefingPages.Count)
                return Error<FlightPlanning>(messages.IncorrectDocumentStructure()).OnError(LogWarn);

            return flightPlanning;
        }
        catch (Exception e)
        {
            throw new FlightPlanningExtractionException(e);
        }
    }

    private void LogWarn(string msg) => logger?.LogDebug(msg);
}