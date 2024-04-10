namespace FlightsExtractor.Extractor;

/* simple factory to  to allow usage without DI */
public class FlightPlanningExtractorFactory
{
    public static IFlightPlanningExtractor Create() => new FlightPlanningExtractor(new OperationalFlightPlanningParser(), new CrewBriefingParser());
}
