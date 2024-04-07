using System.Text.RegularExpressions;

namespace FlightsExtractor.Extractor;
public record FlightPlanning(IEnumerable<Flight> Flights);
public record Flight(OperationalFlightPlan OperationalFlightPlan);
public record OperationalFlightPlan(
    FlightNumber FlightNumber,
    DateOnly FlightDate,
    Result<AircraftRegistration> AircraftRegistration);

public partial record FlightNumber(string Number)
{
    public static Result<FlightNumber> Create(string value)
    {
        if (!FlightNumberRegex().IsMatch(value))
            return Error<FlightNumber>(InvalidFormat($"Flight number must be in format [AA0000], but has value {value}"));

        return Ok(new FlightNumber(value));
    }

    [GeneratedRegex(@"^[A-Z]{2}\d{1,4}$")]
    private static partial Regex FlightNumberRegex();
}

public partial record AircraftRegistration(string Value)
{

    public static Result<AircraftRegistration> Create(string value)
    {
        if (AircraftRegistrationRegex().IsMatch(value))
            return Error<AircraftRegistration>(InvalidFormat($"Aircraft must be in correct format, but has value [{value}]"));

        return Ok(new AircraftRegistration(value));
    }


    // https://gist.github.com/eightyknots/4372d1166a192d5e9754?permalink_comment_id=4469552
    [GeneratedRegex(@"^[A-Z]-[A-Z]{4}|[A-Z]{2}-[A-Z]{3}|N[0-9]{1,5}[A-Z]{0,2}$")]
    private static partial Regex AircraftRegistrationRegex();
}

public class FileDoesNotExistException : Exception;
public class InvalidPdfException(string message, Exception inner) : Exception(message, inner);
public class InvalidPdfStructureException(string message) : Exception(message);