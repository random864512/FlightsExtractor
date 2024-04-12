using System.Collections.Immutable;
using FluentAssertions;
namespace FlightsExtractor.Extractor.Tests;

[TestFixture]
public class FlightPlanningExtractorTests
{
    [Test]
    public void ShouldExtractFlightPlanning()
    {
        // Arrange
        var sut = FlightPlanningExtractorFactory.Create();

        // Act
        var result = sut.Extract("SampleFile.pdf");

        // Assert
        result
            .Should()
            .BeEquivalentTo(Ok(new FlightPlanning([
                new Flight(
                    new FlightNumber("LX1612"),
                    new DateOnly(2024,03,19),
                    new AircraftRegistration("HBJVY"),
                    new Route(new ICAOAirportCode("LSZH"), new ICAOAirportCode("LIMC")),
                    new ICAOAirportCode("LIML"),
                    Error<ICAOAirportCode>("Airdrom 2 is not available"),
                    "SWR612Q",
                    TimeSpan.FromMinutes(48),
                    new decimal(1.7),
                    TimeSpan.FromMinutes(20),
                    new decimal(0.8),
                    new decimal(3.6),
                    ImmutableList.Create(
                        new CrewMember("CMD", "Steve Krebs"),
                        new CrewMember("COP", "Gregory Gillioz"),
                        new CrewMember("CAB", "Luisa Quadros Vissotto"),
                        new CrewMember("CAB", "Rainer Sattler"),
                        new CrewMember("SEN", "Regine Kathrin Schumacher-Horn")
                    )
                ),
                new Flight(
                    new FlightNumber("LX1613"),
                    new DateOnly(2024, 03, 19),
                    new AircraftRegistration("HBJVY"),
                    new Route(new ICAOAirportCode("LIMC"), new ICAOAirportCode("LSZH")),
                    new ICAOAirportCode("LFSB"),
                    Error<ICAOAirportCode>("Airdrom 2 is not available"),
                    "SWR2TM",
                    TimeSpan.FromMinutes(48),
                    new decimal(1.7),
                    TimeSpan.FromMinutes(39),
                    new decimal(1.5),
                    new decimal(4.3),
                    ImmutableList.Create(
                        new CrewMember("CMD", "Steve Krebs"),
                        new CrewMember("COP", "Gregory Gillioz"),
                        new CrewMember("CAB", "Luisa Quadros Vissotto"),
                        new CrewMember("CAB", "Rainer Sattler"),
                        new CrewMember("SEN", "Regine Kathrin Schumacher-Horn")
                    )
                ),
                new Flight(
                    new FlightNumber("LX1072"),
                    new DateOnly(2024, 03, 19),
                    new AircraftRegistration("HBJVN"),
                    new Route(new ICAOAirportCode("LSZH"), new ICAOAirportCode("EDDF")),
                    new ICAOAirportCode("EDDK"),
                    Error<ICAOAirportCode>("Airdrom 2 is not available"),
                    "SWR2ET",
                    TimeSpan.FromMinutes(57),
                    new decimal(1.9),
                    TimeSpan.FromMinutes(47),
                    new decimal(1.6),
                    new decimal(4.6),
                    ImmutableList.Create(
                        new CrewMember("CMD", "Werner Trütsch"),
                        new CrewMember("COP", "Luca Andrea Marchetti"),
                        new CrewMember("CAB", "Helen Meier"),
                        new CrewMember("CAB", "Ena Ramic"),
                        new CrewMember("SEN", "Nico Verhelst")
                    )
                ),
                new Flight(
                    new FlightNumber("LX1073"),
                    new DateOnly(2024, 03, 19),
                    new AircraftRegistration("HBJVN"),
                    new Route(new ICAOAirportCode("EDDF"), new ICAOAirportCode("LSZH")),
                    new ICAOAirportCode("LFSB"),
                    Error<ICAOAirportCode>("Airdrom 2 is not available"),
                    "SWR890M",
                    TimeSpan.FromMinutes(40),
                    new decimal(1.4),
                    TimeSpan.FromMinutes(44),
                    new decimal(1.7),
                    new decimal(4.2),
                    ImmutableList.Create(
                        new CrewMember("CMD", "Werner Trütsch"),
                        new CrewMember("COP", "Luca Andrea Marchetti"),
                        new CrewMember("CAB", "Helen Meier"),
                        new CrewMember("CAB", "Ena Ramic"),
                        new CrewMember("SEN", "Nico Verhelst")
                    )
                )
            ])));
    }

    [Test]
    public void ShouldThrowWhenFileDoesNotExist() =>
    new Action(() => FlightPlanningExtractorFactory.Create().Extract("FileThatDoesNotExists.txt"))
        .Should()
        .Throw<FileDoesNotExistException>();

    [Test]
    public void ShouldReturnErrorWhenBriefingPageIsMissing() =>
        FlightPlanningExtractorFactory.Create().Extract("SampleFileRemovedBriefing.pdf")
            .Should()
            .Be(Error<FlightPlanning>("Briefing page is missing"));

    [Test]
    public void ShouldReturnErrorWhenOperationalFlightPlanningPageIsMissing() =>
        FlightPlanningExtractorFactory.Create().Extract("SampleFileRemovedPlanning.pdf")
            .Should()
            .Be(Error<FlightPlanning>("Incorrect document structure"));
}
