using System.Globalization;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;
using UglyToad.PdfPig;
using UglyToad.PdfPig.DocumentLayoutAnalysis.TextExtractor;

namespace FlightsExtractor.Extractor;

internal record OperationalFlightPage(
    Result<FlightNumber> FlightNumber,
    Result<DateOnly> FlightDate,
    Result<AircraftRegistration> AircraftRegistration,
    Result<ICAOAirportCode> From,
    Result<ICAOAirportCode> To,
    Result<ICAOAirportCode> AlternativeAirdrom1,
    Result<ICAOAirportCode> AlternativeAirdrom2,
    Result<ATCCallSign> ATCCallSign,
    Result<TimeSpan> TimeToDestination,
    Result<decimal> FuelToDestination,
    Result<TimeSpan> TimeToAlternate,
    Result<decimal> FuelToAlternate,
    Result<decimal> MinimumFuelRequired
);

internal partial class OperationalFlightPlanningParser(ILogger<OperationalFlightPlanningParser>? logger = default)
{
    public IEnumerable<OperationalFlightPage> Parse(PdfDocument document)
    {
        logger?.LogDebug("Parsing operational flights");

        foreach (var page in document.GetPages())
        {
            logger?.LogTrace("Parsing page {pageNumber}", page.Number);

            var text = ContentOrderTextExtractor.GetText(page);

            if (page.Text.Contains("Operational Flight Plan"))
            {
                logger?.LogTrace("Planning found on page {pageNumber}", page.Number);
                yield return new OperationalFlightPage(
                    ParseFlightNumber(text),
                    ParseFlightDate(text),
                    ParseAircraftRegistration(text),
                    ParseFrom(text),
                    ParseTo(text),
                    ParseAlternativeAirdrom1(text),
                    ParseAlternativeAirdrom2(text),
                    ParseATCCallSign(text),
                    ParseFuelToAirport(ParseTo(text), text).Time,
                    ParseFuelToAirport(ParseTo(text), text).Fuel,
                    ParseFuelToAirport(ParseAlternativeAirdrom1(text), text).Time,
                    ParseFuelToAirport(ParseAlternativeAirdrom1(text), text).Fuel,
                    ParseFuelMin(text).Fuel
                );
            }

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

    private static Result<ATCCallSign> ParseATCCallSign(string text)
    {
        var match = ATCCallSignRegex().Match(text);

        if (!match.Success)
            return Error<ATCCallSign>(string.Empty);

        return new ATCCallSign(match.Groups["Value"].Value);
    }

    [GeneratedRegex(@"ATC:\s{1,10}(?<Value>\S+)\s+")]
    private static partial Regex ATCCallSignRegex();

    private static (Result<TimeSpan> Time, Result<decimal> Fuel) ParseFuelMin(string text) => ParseFuel("MIN", text);

    private static (Result<TimeSpan> Time, Result<decimal> Fuel) ParseFuelToAirport(Result<ICAOAirportCode> airportResult, string text)
    {
        if (!airportResult.IsSuccess)
            return (Error<TimeSpan>(string.Empty), Error<decimal>(string.Empty));

        return ParseFuel(airportResult.Value!.Value, text);
    }

    private static (Result<TimeSpan> Time, Result<decimal> Fuel) ParseFuel(string field, string text)
    {
        var match = Regex.Match(text, $@"{field}:\s{{1,5}}(?<TimeToDestination>\S+)\s{{1,5}}(?<FuelToDestination>\S+)\s");

        return (TimeToDestination(match), FuelToDestination(match));

        static Result<TimeSpan> TimeToDestination(Match match)
        {
            if (!match.Success)
                return Error<TimeSpan>(string.Empty);

            if (!TimeSpan.TryParse(match.Groups["TimeToDestination"].Value, out var timeToDestination))
                return Error<TimeSpan>(string.Empty);

            return timeToDestination;
        }

        static Result<decimal> FuelToDestination(Match match)
        {
            if (!match.Success)
                return Error<decimal>(string.Empty);

            if (!decimal.TryParse(match.Groups["FuelToDestination"].Value, CultureInfo.InvariantCulture, out var fuelToDestination))
                return Error<decimal>(string.Empty);

            return fuelToDestination;
        }
    }
}