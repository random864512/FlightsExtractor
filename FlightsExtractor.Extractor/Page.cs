namespace FlightsExtractor.Extractor;

public record Page;

public record CrewBriefingPage : Page;

internal record OperationalFlightPage(
    Result<FlightNumber> FlightNumber,
    Result<DateOnly> FlightDate,
    Result<AircraftRegistration> AircraftRegistration,
    Result<ICAOAirportCode> From,
    Result<ICAOAirportCode> To,
    Result<ICAOAirportCode> AlternativeAirdrom1,
    Result<ICAOAirportCode> AlternativeAirdrom2
) : Page;
