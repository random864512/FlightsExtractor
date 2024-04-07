using System.Text.RegularExpressions;

namespace FlightsExtractor.Extractor;
public record FlightPlanning(IEnumerable<Flight> Flights);
public record Flight(OperationalFlightPlan OperationalFlightPlan);
public record OperationalFlightPlan(
    FlightNumber FlightNumber,
    DateOnly FlightDate,
    Result<AircraftRegistration> AircraftRegistration,
    Route Route);

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
        if (!AircraftRegistrationRegex().IsMatch(value))
            return Error<AircraftRegistration>(InvalidFormat($"Aircraft registration must be in correct format, but has value [{value}]"));

        return Ok(new AircraftRegistration(value));
    }


    // TODO: Aircraft registration regex should not be naive
    [GeneratedRegex(@"^[A-Z]{2,10}$")]
    private static partial Regex AircraftRegistrationRegex();
}
public partial record Route(Result<ICAOAirportCode> From, Result<ICAOAirportCode> To);

public partial record ICAOAirportCode(string Value)
{
    public static Result<ICAOAirportCode> Create(string value)
    {
        if (!ICAOAirportCodeRegex().IsMatch(value))
            return Error<ICAOAirportCode>(InvalidFormat($"ICAO Airport must be in correct format, but has value [{value}]"));

        return Ok(new ICAOAirportCode(value));
    }

    // https://en.wikipedia.org/wiki/ICAO_airport_code
    [GeneratedRegex(@"^[A-Z]{4}$")]
    private static partial Regex ICAOAirportCodeRegex();
}

public class FileDoesNotExistException : Exception;
public class InvalidPdfException(string message, Exception inner) : Exception(message, inner);
public class InvalidPdfStructureException(string message) : Exception(message);