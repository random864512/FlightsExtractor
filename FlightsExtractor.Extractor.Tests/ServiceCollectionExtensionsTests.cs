using Microsoft.Extensions.DependencyInjection;
namespace FlightsExtractor.Extractor.Tests;

[TestFixture]
public class ServiceCollectionExtensionsTests
{
    [Test]
    public void ShouldAddFlightPlanningExtractorToServiceCollection()
    {
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddFlightPlanningExtractor();
        using var serviceProvider = serviceCollection.BuildServiceProvider(validateScopes: true);

        var extractor = serviceProvider.GetRequiredService<IFlightPlanningExtractor>();
        extractor.Extract("SampleFile.pdf");
    }
}
