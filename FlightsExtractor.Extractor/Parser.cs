using System.Text.RegularExpressions;
using UglyToad.PdfPig;
using UglyToad.PdfPig.DocumentLayoutAnalysis.TextExtractor;

namespace FlightsExtractor.Extractor;

internal class ParserException(Exception innerException) : Exception(default, innerException);

internal partial class Parser
{
    public IEnumerable<Page> Parse(FileInfo file)
    {
        if (!file.Exists)
            throw new FileDoesNotExistException();

        using var document = PdfDocument.Open(file.FullName);

        foreach (var page in document.GetPages())
        {
            var text = ContentOrderTextExtractor.GetText(page);

            if (page.Text.Contains("Operational Flight Plan"))
                yield return new OperationalFlightPage(
                    ParseFlightNumber(text),
                    ParseFlightDate(text),
                    ParseAircraftRegistration(text),
                    ParseFrom(text),
                    ParseTo(text),
                    ParseAlternativeAirdrom1(text),
                    ParseAlternativeAirdrom2(text)
                );
        }
    }

    private static Result<FlightNumber> ParseFlightNumber(string text)
    {
        var match = FlightNumberRegex().Match(text);
        if (!match.Success)
            return Error<FlightNumber>(string.Empty);

        return FlightNumber.Create(match.Groups["Value"].Value);
    }

    [GeneratedRegex(@"FltNr:\s{1,10}(?<Value>\S+?)\s+")]
    private static partial Regex FlightNumberRegex();

    private static Result<DateOnly> ParseFlightDate(string text)
    {
        var match = FlightDateRegex().Match(text);

        if (!match.Success)
            return Error<DateOnly>(string.Empty);

        if (!DateOnly.TryParseExact(match.Groups["Value"].Value, "ddMMMyy", out var parsedDate))
            return Error<DateOnly>(string.Empty);

        return parsedDate;
    }

    [GeneratedRegex(@"Date:\s{1,10}(?<Value>\S+?)\s+")]
    private static partial Regex FlightDateRegex();

    private static Result<AircraftRegistration> ParseAircraftRegistration(string text)
    {
        var match = AircraftRegistrationRegex().Match(text);
        if (!match.Success)
            return Error<AircraftRegistration>(string.Empty);

        return AircraftRegistration.Create(match.Groups["Value"].Value);
    }

    [GeneratedRegex(@"Reg.:\s{1,10}(?<Value>\S+?)\s+")]
    private static partial Regex AircraftRegistrationRegex();

    private static Result<ICAOAirportCode> ParseFrom(string text)
    {
        var match = FromRegex().Match(text);
        if (!match.Success)
            return Error<ICAOAirportCode>(string.Empty);

        return ICAOAirportCode.Create(match.Groups["Value"].Value);
    }

    [GeneratedRegex(@"From:\s{1,10}(?<Value>\S+?)\s+")]
    private static partial Regex FromRegex();

    private static Result<ICAOAirportCode> ParseTo(string text)
    {
        var match = ToRegex().Match(text);
        if (!match.Success)
            return Error<ICAOAirportCode>(string.Empty);

        return ICAOAirportCode.Create(match.Groups["Value"].Value);
    }

    [GeneratedRegex(@"To:\s{0,10}(?<Value>\S+?)\s+")]
    private static partial Regex ToRegex();

    private static Result<ICAOAirportCode> ParseAlternativeAirdrom1(string text)
    {
        var match = AlternativeAirdrom1Regex().Match(text);
        if (!match.Success)
            return Error<ICAOAirportCode>(string.Empty);

        return ICAOAirportCode.Create(match.Groups["Value"].Value);
    }

    [GeneratedRegex(@"ALTN2:\s{0,10}(?<Value>\S+?)\s+")]
    private static partial Regex AlternativeAirdrom2Regex();

    private static Result<ICAOAirportCode> ParseAlternativeAirdrom2(string text)
    {
        var match = AlternativeAirdrom2Regex().Match(text);
        if (!match.Success)
            return Error<ICAOAirportCode>(string.Empty);

        return ICAOAirportCode.Create(match.Groups["Value"].Value);
    }

    [GeneratedRegex(@"ALTN1:\s{0,10}(?<Value>\S+?)\s+")]
    private static partial Regex AlternativeAirdrom1Regex();
}