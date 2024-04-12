namespace FlightsExtractor.Extractor;
using Factories;
using Microsoft.Extensions.Localization;

/* simple factory to  to allow usage without DI */
public class FlightPlanningExtractorFactory
{
    public static IFlightPlanningExtractor Create(ILoggerFactory? loggerFactory = default, IStringLocalizer<Messages>? stringLocalizer = default) =>
         new FlightPlanningExtractor(
            new FlightNumberFactory(new Messages(stringLocalizer)),
            new AircraftRegistrationFactory(new Messages(stringLocalizer)),
            new ICAOAAirportCodeFactory(new Messages(stringLocalizer)),
            new Messages(stringLocalizer),
            loggerFactory?.CreateLogger<FlightPlanningExtractor>());
}
