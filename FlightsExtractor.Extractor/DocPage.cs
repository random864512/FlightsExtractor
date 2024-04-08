namespace FlightsExtractor.Extractor;

public record DocPage;

public record BriefingPage : DocPage;

internal record PlanPage(
    FlightNumber FlightNumber,
    DateOnly FlightDate,
    Result<AircraftRegistration> AircraftRegistration,
    Result<ICAOAirportCode> From,
    Result<ICAOAirportCode> To,
    Result<ICAOAirportCode> AlternativeAirdrom1,
    Result<ICAOAirportCode> AlternativeAirdrom2
) : DocPage;
