namespace FlightsExtractor.Extractor.Factories;

internal partial class FlightNumberFactory(Messages messages)
{
    public Result<FlightNumber> Create(string value)
    {
        if (!FlightNumberRegex().IsMatch(value))
            return Error<FlightNumber>(messages.IncorrectFormat("flight number", value, "[AA0000]"));

        return new FlightNumber(value);
    }

    [GeneratedRegex(@"^[A-Z]{2}\d{1,4}$")]
    private static partial Regex FlightNumberRegex();
}

internal partial class AircraftRegistrationFactory(Messages messages)
{
    public Result<AircraftRegistration> Create(string value)
    {
        if (!AircraftRegistrationRegex().IsMatch(value))
            return Error<AircraftRegistration>(messages.IncorrectFormat("aircraft registration", value));

        return new AircraftRegistration(value);
    }

    // TODO: Aircraft registration regex should not be naive
    [GeneratedRegex(@"^[A-Z]{2,10}$")]
    private static partial Regex AircraftRegistrationRegex();
}

internal partial class ICAOAAirportCodeFactory(Messages messages)
{
    public Result<ICAOAirportCode> Create(string value)
    {
        if (!ICAOAirportCodeRegex().IsMatch(value))
            return Error<ICAOAirportCode>(messages.IncorrectFormat("ICAO airport code", value));

        return new ICAOAirportCode(value);
    }

    // https://en.wikipedia.org/wiki/ICAO_airport_code
    [GeneratedRegex(@"^[A-Z]{4}$")]
    private static partial Regex ICAOAirportCodeRegex();
}
