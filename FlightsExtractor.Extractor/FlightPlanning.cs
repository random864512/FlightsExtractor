using System.Text.RegularExpressions;

namespace FlightsExtractor.Extractor;
public record FlightPlanning(IEnumerable<Flight> Flights);
public record Flight(OperationalFlightPlan OperationalFlightPlan);
public record OperationalFlightPlan(
    FlightNumber FlightNumber,
    DateOnly FlightDate);

public partial record FlightNumber(string Number)
{
    public static Result<FlightNumber> Create(string value)
    {
        if (!MyRegex().IsMatch(value))
            return Error<FlightNumber>(InvalidFormat("Flight number must be in format [AA0000]"));

        return Ok(new FlightNumber(value));
    }

    [GeneratedRegex(@"[A-Z]{2}\d{1,4}")]
    private static partial Regex MyRegex();
}

public class FileDoesNotExistException : Exception;
public class InvalidPdfException(string message, Exception inner) : Exception(message, inner);
public class InvalidPdfStructureException(string message) : Exception(message);