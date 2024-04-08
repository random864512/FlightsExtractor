using System.Text.RegularExpressions;
using UglyToad.PdfPig.Content;
using UglyToad.PdfPig.DocumentLayoutAnalysis.TextExtractor;

namespace FlightsExtractor.Extractor;

internal class FlightNumberIsMissingException : Exception;
internal class FlightDateIsMissingException : Exception;

internal partial class PlanParser
{
    public PlanPage? Parse(Page page)
    {
        var text = ContentOrderTextExtractor.GetText(page);

        if (page.Text.Contains("Operational Flight Plan"))
            return new PlanPage(
                ParseFlightNumber(text),
                ParseFlightDate(text),
                ParseAircraftRegistration(text),
                ParseFrom(text),
                ParseTo(text),
                ParseAlternativeAirdrom1(text),
                ParseAlternativeAirdrom2(text)
            );

        return default;
    }

    public static FlightNumber ParseFlightNumber(string text)
    {
        var match = FlightNumberRegex().Match(text);
        if (!match.Success)
            throw new FlightNumberIsMissingException();

        var result = FlightNumber.Create(match.Groups["Value"].Value);
        if (!result.IsSuccess)
            throw new FlightNumberIsMissingException();

        return result.Value!;
    }

    [GeneratedRegex(@"FltNr:\s{1,10}(?<Value>\S+?)\s+")]
    private static partial Regex FlightNumberRegex();

    public static DateOnly ParseFlightDate(string text)
    {
        var match = FlightDateRegex().Match(text);

        if (!match.Success)
            throw new FlightDateIsMissingException();

        if (!DateOnly.TryParseExact(match.Groups["Value"].Value, "ddMMMyy", out var parsedDate))
            throw new FlightDateIsMissingException();

        return parsedDate;
    }

    [GeneratedRegex(@"Date:\s{1,10}(?<Value>\S+?)\s+")]
    private static partial Regex FlightDateRegex();

    public static Result<AircraftRegistration> ParseAircraftRegistration(string text)
    {
        var match = AircraftRegistrationRegex().Match(text);
        if (!match.Success)
            return Error<AircraftRegistration>(string.Empty);

        return AircraftRegistration.Create(match.Groups["Value"].Value);
    }

    [GeneratedRegex(@"Reg.:\s{1,10}(?<Value>\S+?)\s+")]
    private static partial Regex AircraftRegistrationRegex();

    public static Result<ICAOAirportCode> ParseFrom(string text)
    {
        var match = FromRegex().Match(text);
        if (!match.Success)
            return Error<ICAOAirportCode>(string.Empty);

        return ICAOAirportCode.Create(match.Groups["Value"].Value);
    }

    [GeneratedRegex(@"From:\s{1,10}(?<Value>\S+?)\s+")]
    private static partial Regex FromRegex();

    public static Result<ICAOAirportCode> ParseTo(string text)
    {
        var match = ToRegex().Match(text);
        if (!match.Success)
            return Error<ICAOAirportCode>(string.Empty);

        return ICAOAirportCode.Create(match.Groups["Value"].Value);
    }

    [GeneratedRegex(@"To:\s{0,10}(?<Value>\S+?)\s+")]
    private static partial Regex ToRegex();

    public static Result<ICAOAirportCode> ParseAlternativeAirdrom1(string text)
    {
        var match = AlternativeAirdrom1Regex().Match(text);
        if (!match.Success)
            return Error<ICAOAirportCode>(string.Empty);

        return ICAOAirportCode.Create(match.Groups["Value"].Value);
    }

    [GeneratedRegex(@"ALTN2:\s{0,10}(?<Value>\S+?)\s+")]
    private static partial Regex AlternativeAirdrom2Regex();

    public static Result<ICAOAirportCode> ParseAlternativeAirdrom2(string text)
    {
        var match = AlternativeAirdrom2Regex().Match(text);
        if (!match.Success)
            return Error<ICAOAirportCode>(string.Empty);

        return ICAOAirportCode.Create(match.Groups["Value"].Value);
    }

    [GeneratedRegex(@"ALTN1:\s{0,10}(?<Value>\S+?)\s+")]
    private static partial Regex AlternativeAirdrom1Regex();
}