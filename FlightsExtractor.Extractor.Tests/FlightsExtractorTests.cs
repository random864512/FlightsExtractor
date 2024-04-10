using FluentAssertions;
namespace FlightsExtractor.Extractor.Tests;

[TestFixture]
public class FlightPlanningExtractorTests
{
    [Test]
    public void ShouldExtractFlightPlanning()
    {
        // Arrange
        var sut = new FlightPlanningExtractor(new OperationalFlightPlanningParser(), new CrewBriefingParser());

        // Act
        var result = sut.Extract("SampleFile.pdf");

        // Assert
        result
            .Should()
            .BeEquivalentTo(new FlightPlanning([
                new Flight(
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
                    new decimal(3.6),
                    [
                        new CrewMember("CMD", "Steve Krebs"),
                        new CrewMember("COP", "Gregory Gillioz"),
                        new CrewMember("CAB", "Luisa Quadros Vissotto"),
                        new CrewMember("CAB", "Rainer Sattler"),
                        new CrewMember("SEN", "Regine Kathrin Schumacher-Horn")
                    ]
                ),
                new Flight(
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
                    new decimal(4.3),
                    [
                        new CrewMember("CMD", "Steve Krebs"),
                        new CrewMember("COP", "Gregory Gillioz"),
                        new CrewMember("CAB", "Luisa Quadros Vissotto"),
                        new CrewMember("CAB", "Rainer Sattler"),
                        new CrewMember("SEN", "Regine Kathrin Schumacher-Horn")
                    ]
                ),
                new Flight(
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
                    new decimal(4.6),
                    [
                        new CrewMember("CMD", "Werner Trütsch"),
                        new CrewMember("COP", "Luca Andrea Marchetti"),
                        new CrewMember("CAB", "Helen Meier"),
                        new CrewMember("CAB", "Ena Ramic"),
                        new CrewMember("SEN", "Nico Verhelst")
                    ]
                ),
                new Flight(
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
                    new decimal(4.2),
                     [
                        new CrewMember("CMD", "Werner Trütsch"),
                        new CrewMember("COP", "Luca Andrea Marchetti"),
                        new CrewMember("CAB", "Helen Meier"),
                        new CrewMember("CAB", "Ena Ramic"),
                        new CrewMember("SEN", "Nico Verhelst"),

                    ]
                )
            ]));
    }

    [Test]
    public void ShouldThrowWhenFileDoesNotExist() =>
    new Action(() => new FlightPlanningExtractor(new OperationalFlightPlanningParser(), new CrewBriefingParser())
        .Extract("FileThatDoesNotExists.txt"))
        .Should()
        .Throw<FileDoesNotExistException>();


    [Test]
    public void ShouldThrowWhenBriefingPageIsMissing() =>
        new Action(() => new FlightPlanningExtractor(new OperationalFlightPlanningParser(), new CrewBriefingParser())
        .Extract("SampleFileRemovedBriefing.pdf"))
        .Should()
        .Throw<FlightPlanningValidationException>();

    [Test]
    public void ShouldThrowWhenOperationalFlightPlanningPageIsMissing() =>
        new Action(() => new FlightPlanningExtractor(new OperationalFlightPlanningParser(), new CrewBriefingParser())
        .Extract("SampleFileRemovedPlanning.pdf"))
        .Should()
        .Throw<FlightPlanningValidationException>();
}
