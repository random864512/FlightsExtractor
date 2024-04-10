using Microsoft.Extensions.Logging;

namespace FlightsExtractor.Extractor;

/* simple factory to  to allow usage without DI */
public class FlightPlanningExtractorFactory
{
    public static IFlightPlanningExtractor Create(ILoggerFactory? loggerFactory = default)
    {
        return new FlightPlanningExtractor(
            new OperationalFlightPlanningParser(loggerFactory?.CreateLogger<OperationalFlightPlanningParser>()),
            new CrewBriefingParser(loggerFactory?.CreateLogger<CrewBriefingParser>()),
            loggerFactory?.CreateLogger<FlightPlanningExtractor>());
    }
}
