using System.Collections.Immutable;
using UglyToad.PdfPig;

namespace FlightsExtractor.Extractor;

public interface IFlightPlanningExtractor
{
    FlightPlanning Extract(string file);
}

internal class FlightPlanningExtractor(
    OperationalFlightPlanningParser operationalFlightPlanningParser,
    CrewBriefingParser crewBriefingParser)
    : IFlightPlanningExtractor
{
    public FlightPlanning Extract(string file)
    {
        if (!File.Exists(file))
            throw new FileDoesNotExistException();

        using var document = PdfDocument.Open(file);

        var operationalFlightPages = operationalFlightPlanningParser.Parse(document).ToImmutableList();
        var crewBriefingPages = crewBriefingParser.Parse(document).ToImmutableList();

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