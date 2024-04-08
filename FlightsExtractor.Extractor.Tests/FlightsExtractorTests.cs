using FluentAssertions;
using UglyToad.PdfPig;

namespace FlightsExtractor.Extractor.Tests;

[TestFixture]
public class FlightPlanningExtractorTests
{
    [Test]
    public void ShouldExtractFlightPlanning()
    {
        using var pdf = PdfDocument.Open("SampleFile.pdf");

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
                    Ok(new AircraftRegistration("HBJVY")),
                    new Route(Ok(new ICAOAirportCode("LSZH")), Ok(new ICAOAirportCode("LIMC"))),
                    Ok(new ICAOAirportCode("LIML")),
                    Error<ICAOAirportCode>(string.Empty),
                    Ok(new ATCCallSign("SWR612Q"))
                )),
                new Flight(new OperationalFlightPlan(
                    new FlightNumber("LX1613"),
                    new DateOnly(2024,03,19),
                    Ok(new AircraftRegistration("HBJVY")),
                    new Route(Ok(new ICAOAirportCode("LIMC")), Ok(new ICAOAirportCode("LSZH"))),
                    Ok(new ICAOAirportCode("LFSB")),
                    Error<ICAOAirportCode>(string.Empty),
                    Ok(new ATCCallSign("SWR2TM"))
                )),
                new Flight(new OperationalFlightPlan(
                    new FlightNumber("LX1072"),
                    new DateOnly(2024, 03, 19),
                    Ok(new AircraftRegistration("HBJVN")),
                    new Route(Ok(new ICAOAirportCode("LSZH")), Ok(new ICAOAirportCode("EDDF"))),
                    Ok(new ICAOAirportCode("EDDK")),
                    Error<ICAOAirportCode>(string.Empty),
                    Ok(new ATCCallSign("SWR2ET"))
                )),
                new Flight(new OperationalFlightPlan(
                    new FlightNumber("LX1073"),
                    new DateOnly(2024, 03, 19),
                    Ok(new AircraftRegistration("HBJVN")),
                    new Route(Ok(new ICAOAirportCode("EDDF")), Ok(new ICAOAirportCode("LSZH"))),
                    Ok(new ICAOAirportCode("LFSB")),
                    Error<ICAOAirportCode>(string.Empty),
                    Ok(new ATCCallSign("SWR890M"))
                ))
            ]));
    }
}
