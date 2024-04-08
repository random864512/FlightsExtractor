using System.Text.RegularExpressions;

namespace FlightsExtractor.Extractor;
public record FlightPlanning(IEnumerable<Flight> Flights);
public record Flight(OperationalFlightPlan OperationalFlightPlan);
public record OperationalFlightPlan(
    FlightNumber FlightNumber,
    DateOnly FlightDate,
    Result<AircraftRegistration> AircraftRegistration,
    Route Route,
    Result<ICAOAirportCode> AlternativeAirdrom1,
    Result<ICAOAirportCode> AlternativeAirdrom2);

public partial record FlightNumber(string Number)
{
    public static Result<FlightNumber> Create(string value)
    {
        if (!FlightNumberRegex().IsMatch(value))
            return Error<FlightNumber>($"Flight number must be in format [AA0000], but has value {value}");

        return new FlightNumber(value);
    }

    [GeneratedRegex(@"^[A-Z]{2}\d{1,4}$")]
    private static partial Regex FlightNumberRegex();
}

public partial record AircraftRegistration(string Value)
{

    public static Result<AircraftRegistration> Create(string value)
    {
        if (!AircraftRegistrationRegex().IsMatch(value))
            return Error<AircraftRegistration>($"Aircraft registration must be in correct format, but has value [{value}]");

        return new AircraftRegistration(value);
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
            return Error<ICAOAirportCode>($"ICAO Airport must be in correct format, but has value [{value}]");

        return new ICAOAirportCode(value);
    }

    // https://en.wikipedia.org/wiki/ICAO_airport_code
    [GeneratedRegex(@"^[A-Z]{4}$")]
    private static partial Regex ICAOAirportCodeRegex();
}