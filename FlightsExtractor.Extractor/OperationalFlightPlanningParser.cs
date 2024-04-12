using System.Globalization;

namespace FlightsExtractor.Extractor;

internal partial class OperationalFlightPlanningPage(PdfPage page)
{
    public static OperationalFlightPlanningPage? Create(PdfPage page)
    {
        if (page.Text().Contains("Operational Flight Plan"))
            return new OperationalFlightPlanningPage(page);
        return default;
    }

    public string? ParseFlightNumber()
    {
        var match = FlightNumberRegex().Match(page.ContentOrderedText());
        return match.Success ? match.Groups["Value"].Value : default;
    }

    [GeneratedRegex(@"FltNr:\s{1,10}(?<Value>\S+?)\s+")]
    private static partial Regex FlightNumberRegex();

    public DateOnly? ParseFlightDate()
    {
        var match = FlightDateRegex().Match(page.ContentOrderedText());

        if (!match.Success)
            return default;

        if (!DateOnly.TryParseExact(match.Groups["Value"].Value, "ddMMMyy", out var parsedDate))
            return default;

        return parsedDate;
    }

    [GeneratedRegex(@"Date:\s{1,10}(?<Value>\S+?)\s+")]
    private static partial Regex FlightDateRegex();

    public string? ParseAircraftRegistration()
    {
        var match = AircraftRegistrationRegex().Match(page.ContentOrderedText());
        return !match.Success ? default : match.Groups["Value"].Value;
    }

    [GeneratedRegex(@"Reg.:\s{1,10}(?<Value>\S+?)\s+")]
    private static partial Regex AircraftRegistrationRegex();

    public string? ParseFrom()
    {
        var match = FromRegex().Match(page.ContentOrderedText());
        return !match.Success ? default : match.Groups["Value"].Value;
    }

    [GeneratedRegex(@"From:\s{1,10}(?<Value>\S+?)\s+")]
    private static partial Regex FromRegex();

    public string? ParseTo()
    {
        var match = ToRegex().Match(page.ContentOrderedText());
        return !match.Success ? default : match.Groups["Value"].Value;
    }

    [GeneratedRegex(@"To:\s{0,10}(?<Value>\S+?)\s+")]
    private static partial Regex ToRegex();

    public string? ParseAlternativeAirdrom1()
    {
        var match = AlternativeAirdrom1Regex().Match(page.ContentOrderedText());
        return !match.Success ? default : match.Groups["Value"].Value;
    }

    [GeneratedRegex(@"ALTN2:\s{0,10}(?<Value>\S+?)\s+")]
    private static partial Regex AlternativeAirdrom2Regex();

    public string? ParseAlternativeAirdrom2()
    {
        var match = AlternativeAirdrom2Regex().Match(page.ContentOrderedText());
        return !match.Success ? default : match.Groups["Value"].Value;
    }

    [GeneratedRegex(@"ALTN1:\s{0,10}(?<Value>\S+?)\s+")]
    private static partial Regex AlternativeAirdrom1Regex();

    public string? ParseATCCallSign()
    {
        var match = ATCCallSignRegex().Match(page.ContentOrderedText());
        return !match.Success ? default : match.Groups["Value"].Value;
    }

    [GeneratedRegex(@"ATC:\s{1,10}(?<Value>\S+)\s+")]
    private static partial Regex ATCCallSignRegex();

    public decimal? ParseFuelMin() => ParseFuel("MIN")?.Fuel;

    public (TimeSpan? Time, decimal? Fuel)? ParseFuelToAirport(string airport) => ParseFuel(airport);

    public (TimeSpan? Time, decimal? Fuel)? ParseFuel(string field)
    {
        var match = Regex.Match(page.ContentOrderedText(), $@"{field}:\s{{1,5}}(?<TimeToDestination>\S+)\s{{1,5}}(?<FuelToDestination>\S+)\s");
        if (!match.Success)
            return default;

        return (TimeToDestination(match), FuelToDestination(match));

        static TimeSpan? TimeToDestination(Match match)
        {
            if (!TimeSpan.TryParse(match.Groups["TimeToDestination"].Value, out var timeToDestination))
                return default;

            return timeToDestination;
        }

        static decimal? FuelToDestination(Match match)
        {
            if (!decimal.TryParse(match.Groups["FuelToDestination"].Value, CultureInfo.InvariantCulture, out var fuelToDestination))
                return default;

            return fuelToDestination;
        }
    }
}