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
                ParseFlightDate(page)
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
}