using System.Collections.Immutable;
using Microsoft.Extensions.Logging;
using UglyToad.PdfPig;

namespace FlightsExtractor.Extractor;

/*
    public interface representing pdf extractor
    depending on needs (like passing lib to 3rd parties, public nuget etc.) to completely avoid eventual changes due to implementation details it could look like:
    public interface IFlightPlanningExtractor : IDisposable, IAsyncDisposable
    {
        Task<FlightPlanning> Extract(string file);
    }
    to avoid breaking changes in case of library switch in the future
 */

public interface IFlightPlanningExtractor
{
    /// <exception cref="FileDoesNotExistException">File does not exist</exception>
    /// <exception cref="FlightPlanningValidationException">File cannot be parsed due to invalid file structure / missing data</exception>
    /// <exception cref="FlightPlanningExtractionException">File cannot be parsed due to internal error or invalid file format</exception>
    FlightPlanning Extract(string file);
}

internal class FlightPlanningExtractor(
    OperationalFlightPlanningParser operationalFlightPlanningParser,
    CrewBriefingParser crewBriefingParser,
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
            using var document = PdfDocument.Open(file);

            logger?.LogDebug("Parsing file");
            var operationalFlightPages = operationalFlightPlanningParser.Parse(document).ToImmutableList();
            var crewBriefingPages = crewBriefingParser.Parse(document).ToImmutableList();

            logger?.LogDebug("Validating file");
            var error = Validate(operationalFlightPages, crewBriefingPages);
            if (error != default)
                throw new FlightPlanningValidationException(error);

            var flights = operationalFlightPages.Select(plan => (plan, briefing: crewBriefingPages.Single(briefing => briefing.FlightNumber!.Value!.Number.Equals(plan.FlightNumber!.Value!.Number))));

            return new FlightPlanning(flights.Select(flight =>
                new Flight(
                        flight.plan.FlightNumber.Value!,
                        flight.plan.FlightDate.Value!,
                        flight.plan.AircraftRegistration,
                        new Route(flight.plan.From, flight.plan.To),
                        flight.plan.AlternativeAirdrom1,
                        flight.plan.AlternativeAirdrom2,
                        flight.plan.ATCCallSign,
                        flight.plan.TimeToDestination,
                        flight.plan.FuelToDestination,
                        flight.plan.TimeToAlternate,
                        flight.plan.FuelToAlternate,
                        flight.plan.MinimumFuelRequired,
                        flight.briefing.CrewMembers
                )).ToImmutableList());
        }
        catch (Exception e) when (e is not FlightPlanningValidationException)
        {
            throw new FlightPlanningExtractionException(e);
        }
    }

    private static string? Validate(ImmutableList<OperationalFlightPage> plans, ImmutableList<CrewBriefingPage> briefings)
    {
        if (plans.Select(x => x.FlightNumber).Any(x => !x.IsSuccess))
            return "Missing flight number in operational flight page";

        if (plans.Select(x => x.FlightDate).Any(x => !x.IsSuccess))
            return "Missing flight date in operational flight page";

        if (briefings.Select(x => x.FlightNumber).Any(x => !x.IsSuccess))
            return "Missing flight number in crew briefing page";

        if (!plans.Select(x => x.FlightNumber.Value!.Number).OrderBy(x => x).SequenceEqual(briefings.Select(x => x.FlightNumber.Value!.Number).OrderBy(x => x)))
            return "Flights numbers in plan and briefings does not match";

        return default;
    }
}