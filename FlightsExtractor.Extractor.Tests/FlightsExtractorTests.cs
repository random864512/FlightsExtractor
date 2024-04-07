using FluentAssertions;

namespace FlightsExtractor.Extractor.Tests;

[TestFixture]
public class FlightsExtractorTests
{
    [Test]
    public void ShouldExtractFlightsData()
    {
        // Arrange
        var sut = new FlightsExtractor();
        var file = new FileInfo("SampleFile.pdf");

        // Act && Assert
        var result = sut.Extract(file);

        //Assert
        result
            .Should()
            .BeEquivalentTo(new Document([
                new Flight(new OperationalFlightPlan(new FlightNumber("LX1612"))),
                new Flight(new OperationalFlightPlan(new FlightNumber("LX1613"))),
                new Flight(new OperationalFlightPlan(new FlightNumber("LX1072"))),
                new Flight(new OperationalFlightPlan(new FlightNumber("LX1073")))
            ]));
    }
}
