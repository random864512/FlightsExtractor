using Microsoft.Extensions.DependencyInjection;
namespace FlightsExtractor.Extractor.Tests;

[TestFixture]
public class ServiceCollectionExtensionsTests
{
    public void ShouldAddFlightPlanningExtractorToServiceCollection()
    {
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddFlightPlanningExtractor();
        using var serviceProvider = serviceCollection.BuildServiceProvider();

        var extractor = serviceProvider.GetRequiredService<IFlightPlanningExtractor>();
        extractor.Extract("SampleFile.pdf");
    }
}
