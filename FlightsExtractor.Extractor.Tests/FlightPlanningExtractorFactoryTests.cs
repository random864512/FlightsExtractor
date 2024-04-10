namespace FlightsExtractor.Extractor.Tests;

[TestFixture]
public class FlightPlanningExtractorFactoryTests
{
    [Test]
    public void ShouldCreateFlightExtractor() => FlightPlanningExtractorFactory.Create().Extract("SampleFile.pdf");
}
