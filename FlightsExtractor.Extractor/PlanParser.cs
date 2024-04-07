using UglyToad.PdfPig.Content;

namespace FlightsExtractor.Extractor;

internal class FlightNumberIsMissingException : Exception;
internal class FlightDateIsMissingException : Exception;

internal partial class PlanParser
{
    public PlanPage? Parse(Page page)
    {
        if (page.Text.Contains("Operational Flight Plan"))
            return new PlanPage(
                ParseFlightNumber(page),
                ParseFlightDate(page),
                ParseAircraftRegistration(page),
                ParseFrom(page),
                ParseTo(page)
            );

        return default;
    }

    public static FlightNumber ParseFlightNumber(Page page)
    {
        var result = page.GetWordAfter("FltNr:").Map(FlightNumber.Create);
        if (!result.IsSuccess)
            throw new FlightNumberIsMissingException();

        return result.Value!;
    }

    public static DateOnly ParseFlightDate(Page page)
    {
        var parsed = page.GetWordAfter("Date:");

        if (parsed.IsSuccess && DateOnly.TryParseExact(parsed.Value, "ddMMMyy", out var parsedDate))
            return parsedDate;

        throw new FlightDateIsMissingException();
    }

    public static Result<AircraftRegistration> ParseAircraftRegistration(Page page) =>
        page.GetWordAfter("Reg.:").Map(AircraftRegistration.Create);

    public static Result<ICAOAirportCode> ParseFrom(Page page) =>
        page.GetWordAfter("From:").Map(ICAOAirportCode.Create);

    public static Result<ICAOAirportCode> ParseTo(Page page) =>
        page.GetWordAfter("To:").Map(ICAOAirportCode.Create);
}