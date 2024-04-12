using FlightsExtractor.Extractor.Factories;
using UglyToad.PdfPig;

namespace FlightsExtractor.Extractor;

/*
    public interface representing pdf extractor
    depending on needs (like passing lib to 3rd parties, public nuget etc.) to completely avoid eventual changes due to implementation details it could look like:
    public interface IFlightPlanningExtractor : IDisposable, IAsyncDisposable
    {
        Task<FlightPlanning> Extract(string file, CancellationToken token);
    }
    to avoid breaking changes in case of library switch or update in the future
 */

public interface IFlightPlanningExtractor
{
    /// <exception cref="FileDoesNotExistException">File does not exist</exception>
    /// <exception cref="FlightPlanningValidationException">File cannot be parsed due to invalid file structure / missing data</exception>
    /// <exception cref="FlightPlanningExtractionException">File cannot be parsed due to internal error or invalid file format</exception>
    FlightPlanning Extract(string file);
}

internal class FlightPlanningExtractor(
    FlightNumberFactory flightNumberFactory,
    AircraftRegistrationFactory aircraftRegistrationFactory,
    ICAOAAirportCodeFactory airportCodeFactory,
    Messages messages,
    ILogger<FlightPlanningExtractor>? logger = default)
    : IFlightPlanningExtractor
{
    public FlightPlanning Extract(string file)
    {
        logger?.LogDebug("Checking if file exists");
        if (!File.Exists(file))
            throw new FileDoesNotExistException();

        try
        {
            logger?.LogDebug("Opening file");
            using var document = FlightPlanningDocument.Create(file);

            FlightPlanning flightPlanning = new([]);
            foreach (var planning in document.PlanningPages)
            {
                var flightNumber = planning.ParseFlightNumber().ToResult(error: messages.CannotParse("planning flight number")).MapResult(flightNumberFactory.Create);
                if (!flightNumber.IsSuccess)
                    throw new FlightPlanningValidationException(flightNumber.Error!);

                var flightDate = planning.ParseFlightDate().ToResult(error: messages.CannotParse("planning flight number"));
                if (!flightDate.IsSuccess)
                    throw new FlightPlanningValidationException(flightNumber.Error!);

                Result<CrewBriefingPage> briefing = document.BriefingPages.FirstOrDefault(x => flightNumber.Value!.Number.Equals(x.ParseCrewTable()?.flightNumber)).ToResult(messages.CannotParse("briefing flight number"));
                if (!briefing.IsSuccess)
                    throw new FlightPlanningValidationException(briefing.Error!);

                flightPlanning = flightPlanning with
                {
                    Flights = [ .. flightPlanning.Flights,
                    new Flight(
                        flightNumber!,
                        flightDate,
                        planning.ParseAircraftRegistration().ToResult(messages.CannotParse("aircraft registration")).MapResult(aircraftRegistrationFactory.Create),
                        new Route(
                            planning.ParseFrom().ToResult(messages.CannotParse("from")).MapResult(airportCodeFactory.Create),
                            planning.ParseTo().ToResult(messages.CannotParse("to")).MapResult(airportCodeFactory.Create)
                        ),
                        planning.ParseAlternativeAirdrom1().ToResult(messages.CannotParse("alternative airdrom 1")).MapResult(airportCodeFactory.Create),
                        planning.ParseAlternativeAirdrom2().ToResult(messages.CannotParse("alternative airdrom 2")).MapResult(airportCodeFactory.Create),
                        planning.ParseATCCallSign().ToResult(messages.CannotParse("ATC call sign")),
                        (planning.ParseTo().MapNullable(planning.ParseFuelToAirport)?.Time).ToResult(messages.CannotParse("time to destination")),
                        (planning.ParseTo().MapNullable(planning.ParseFuelToAirport)?.Fuel).ToResult(messages.CannotParse("fuel to destination")),
                        (planning.ParseAlternativeAirdrom1().MapNullable(planning.ParseFuelToAirport)?.Time).ToResult(messages.CannotParse("time to alternative")),
                        (planning.ParseAlternativeAirdrom1().MapNullable(planning.ParseFuelToAirport)?.Fuel).ToResult(messages.CannotParse("fuel to alternative")),
                        planning.ParseFuelMin().ToResult(messages.CannotParse("fuel min")),
                        (briefing.Value!.ParseCrewTable()?.crewMembers?.Select(parsed =>
                            new CrewMember(parsed.function.ToResult(messages.CannotParse("crew member function")),
                            parsed.name.ToResult(messages.CannotParse("crew member name")))).ToImmutableList()).ToResult(messages.CannotParse("crew member list"))
                    )
                    ]
                };
            }

            if (flightPlanning.Flights.Count() != document.PlanningPages.Count() ||
                flightPlanning.Flights.Count() != document.BriefingPages.Count())
                throw new FlightPlanningValidationException(messages.IncorrectDocumentStructure());

            return flightPlanning;
        }
        catch (Exception e) when (e is not FlightPlanningValidationException)
        {
            throw new FlightPlanningExtractionException(e);
        }
    }

}