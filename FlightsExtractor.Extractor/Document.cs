namespace FlightsExtractor.Extractor;

public record Document(IEnumerable<Flight> Flights);
public record Flight(OperationalFlightPlan OperationalFlightPlan);
public record OperationalFlightPlan(FlightNumber FlightNumber);
public record FlightNumber(string Number);