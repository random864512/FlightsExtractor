using FluentAssertions;

namespace FlightsExtractor.Extractor.Tests;

[TestFixture]
public class FlightPlanningExtractorTests
{
    [Test]
    public void ShouldExtractFlightPlanning()
    {
        // Arrange
        var sut = new FlightPlanningExtractor();
        var file = new FileInfo("SampleFile.pdf");

        // Act
        var result = sut.Extract(file);

        // Assert
        result
            .Should()
            .BeEquivalentTo(new FlightPlanning([
                new Flight(new OperationalFlightPlan(new FlightNumber("LX1612"))),
                new Flight(new OperationalFlightPlan(new FlightNumber("LX1613"))),
                new Flight(new OperationalFlightPlan(new FlightNumber("LX1072"))),
                new Flight(new OperationalFlightPlan(new FlightNumber("LX1073")))
            ]));
    }
}
