using FluentAssertions;
using UglyToad.PdfPig;

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
                new Flight(new OperationalFlightPlan(
                    new FlightNumber("LX1612"),
                    new DateOnly(2024,03,19))),
                new Flight(new OperationalFlightPlan(
                    new FlightNumber("LX1613"),
                      new DateOnly(2024,03,19))),
                new Flight(new OperationalFlightPlan(
                    new FlightNumber("LX1072"),
                      new DateOnly(2024,03,19))),
                new Flight(new OperationalFlightPlan(
                    new FlightNumber("LX1073"),
                      new DateOnly(2024,03,19)))
            ]));
    }
}
