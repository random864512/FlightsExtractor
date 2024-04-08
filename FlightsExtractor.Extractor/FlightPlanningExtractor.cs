using System.Collections.Immutable;

namespace FlightsExtractor.Extractor;

public interface IFlightPlanningExtractor
{
    FlightPlanning Extract(FileInfo file);
}

public class FileDoesNotExistException : Exception;
public class FlightPlanningValidationException(string message) : Exception(message);


internal class FlightPlanningExtractor(Parser parser) : IFlightPlanningExtractor
{
    public FlightPlanning Extract(FileInfo file)
    {
        var pages = parser.Parse(file).ToImmutableList();

        return new FlightPlanning(pages.OfType<OperationalFlightPage>().Select(plan =>
            new Flight(
                new OperationalFlightPlan(
                    plan.FlightNumber.IsSuccess ? plan.FlightNumber.Value! : throw new FlightPlanningValidationException("Missing flightNumber"),
                    plan.FlightDate.IsSuccess ? plan.FlightDate.Value! : throw new FlightPlanningValidationException("Missing flight date"),
                    plan.AircraftRegistration,
                    new Route(plan.From, plan.To),
                    plan.AlternativeAirdrom1,
                    plan.AlternativeAirdrom2,
                    plan.ATCCallSign
                )
            )));
    }
}