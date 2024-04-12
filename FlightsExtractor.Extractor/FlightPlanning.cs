namespace FlightsExtractor.Extractor;

public class FileDoesNotExistException : Exception;
public class FlightPlanningValidationException(string message) : Exception(message);
public class FlightPlanningExtractionException(Exception inner) : Exception(default, inner);

public record FlightPlanning(ImmutableList<Flight> Flights);

public record Flight(
    FlightNumber FlightNumber,
    DateOnly FlightDate,
    Result<AircraftRegistration> AircraftRegistration,
    Route Route,
    Result<ICAOAirportCode> AlternativeAirdrom1,
    Result<ICAOAirportCode> AlternativeAirdrom2,
    Result<string> ATCCallSign,
    Result<TimeSpan> TimeToDestination,
    Result<decimal> FuelToDestination,
    Result<TimeSpan> TimeToAlternate,
    Result<decimal> FuelToAlternate,
    Result<decimal> MinimumFuelRequired,
    Result<ImmutableList<CrewMember>> CrewMembers
);

public record FlightNumber(string Number);
public record ICAOAirportCode(string Value);
public record AircraftRegistration(string Value);
public record Route(Result<ICAOAirportCode> From, Result<ICAOAirportCode> To);
public record CrewMember(Result<string> Function, Result<string> Name);