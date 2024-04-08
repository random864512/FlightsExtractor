using FluentAssertions;

namespace FlightsExtractor.Extractor.Tests;

[TestFixture]
public class FlightPlanningExtractorTests
{
    [Test]
    public void ShouldExtractFlightPlanning()
    {
        // Arrange
        var sut = new FlightPlanningExtractor(new Parser());
        var file = new FileInfo("SampleFile.pdf");

        // Act
        var result = sut.Extract(file);

        // Assert
        result
            .Should()
            .BeEquivalentTo(new FlightPlanning([
                new Flight(new OperationalFlightPlan(
                    new FlightNumber("LX1612"),
                    new DateOnly(2024,03,19),
                    new AircraftRegistration("HBJVY"),
                    new Route(new ICAOAirportCode("LSZH"), new ICAOAirportCode("LIMC")),
                    new ICAOAirportCode("LIML"),
                    Error<ICAOAirportCode>(string.Empty),
                    new ATCCallSign("SWR612Q"),
                    TimeSpan.FromMinutes(48),
                    new decimal(1.7),
                    TimeSpan.FromMinutes(20),
                    new decimal(0.8),
                    new decimal(3.6)
                )),
                new Flight(new OperationalFlightPlan(
                    new FlightNumber("LX1613"),
                    new DateOnly(2024, 03, 19),
                    new AircraftRegistration("HBJVY"),
                    new Route(new ICAOAirportCode("LIMC"), new ICAOAirportCode("LSZH")),
                    new ICAOAirportCode("LFSB"),
                    Error<ICAOAirportCode>(string.Empty),
                    new ATCCallSign("SWR2TM"),
                    TimeSpan.FromMinutes(48),
                    new decimal(1.7),
                    TimeSpan.FromMinutes(39),
                    new decimal(1.5),
                    new decimal(4.3)
                )),
                new Flight(new OperationalFlightPlan(
                    new FlightNumber("LX1072"),
                    new DateOnly(2024, 03, 19),
                    new AircraftRegistration("HBJVN"),
                    new Route(new ICAOAirportCode("LSZH"), new ICAOAirportCode("EDDF")),
                    new ICAOAirportCode("EDDK"),
                    Error<ICAOAirportCode>(string.Empty),
                    new ATCCallSign("SWR2ET"),
                    TimeSpan.FromMinutes(57),
                    new decimal(1.9),
                    TimeSpan.FromMinutes(47),
                    new decimal(1.6),
                    new decimal(4.6)
                )),
                new Flight(new OperationalFlightPlan(
                    new FlightNumber("LX1073"),
                    new DateOnly(2024, 03, 19),
                    new AircraftRegistration("HBJVN"),
                    new Route(new ICAOAirportCode("EDDF"), new ICAOAirportCode("LSZH")),
                    new ICAOAirportCode("LFSB"),
                    Error<ICAOAirportCode>(string.Empty),
                    new ATCCallSign("SWR890M"),
                    TimeSpan.FromMinutes(40),
                    new decimal(1.4),
                    TimeSpan.FromMinutes(44),
                    new decimal(1.7),
                    new decimal(4.2)
                ))
            ]));
    }
}
