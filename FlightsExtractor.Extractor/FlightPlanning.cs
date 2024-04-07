namespace FlightsExtractor.Extractor;

public record FlightPlanning(IEnumerable<Flight> Flights);
public record Flight(OperationalFlightPlan OperationalFlightPlan);
public record OperationalFlightPlan(
    FlightNumber FlightNumber,
    DateOnly FlightDate);

public record FlightNumber(string Number);

public class FileDoesNotExistException : Exception;
public class InvalidPdfException(string message, Exception inner) : Exception(message, inner);

public abstract class InvalidPdfStructureException(Exception? inner = default) : Exception(string.Empty, inner);
public class MissingOperationalFlightPlanOrCrewBriefingException : InvalidPdfStructureException;
public class MissingFlightNumberException(Exception inner) : InvalidPdfStructureException(inner);
public class MissingFlightDateException(Exception inner) : InvalidPdfStructureException(inner);