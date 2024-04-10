using Microsoft.Extensions.DependencyInjection;

namespace FlightsExtractor.Extractor;

public static class ServiceCollectionExtensions
{
    /* Registers IFlightsExtractor in Microsoft Dependency Injection container */
    public static IServiceCollection AddFlightPlanningExtractor(this IServiceCollection serviceCollection) =>
        serviceCollection
            .AddSingleton<IFlightPlanningExtractor, FlightPlanningExtractor>()
            .AddSingleton<CrewBriefingParser>()
            .AddSingleton<OperationalFlightPlanningParser>();
}
