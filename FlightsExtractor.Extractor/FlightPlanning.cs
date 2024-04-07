namespace FlightsExtractor.Extractor;

public record FlightPlanning(IEnumerable<Flight> Flights);
public record Flight(OperationalFlightPlan OperationalFlightPlan);
public record OperationalFlightPlan(FlightNumber FlightNumber);
public record FlightNumber(string Number);

public class FileDoesNotExistException : Exception;
public class InvalidPdfException(string message, Exception inner) : Exception(message, inner);
public class InvalidPdfStructureException(string message) : Exception(message);