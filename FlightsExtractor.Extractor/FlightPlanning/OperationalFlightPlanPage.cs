using System.Text.RegularExpressions;
using MoreLinq;
using UglyToad.PdfPig.Content;

namespace FlightsExtractor.Extractor;

internal partial class OperationalFlightPlanPage(Page Page)
{
    public static bool IsOperationalFlightPlanPage(Page page) => page.Text.Contains("Operational Flight Plan");

    public string FlightNumber()
    {
        try
        {
            return FlightNumberRegex().Match(Page.Text).Groups["FlightNumber"].Value;
        }
        catch (Exception e)
        {
            throw new MissingFlightNumberException(e);
        }
    }

    [GeneratedRegex(@"FltNr:\s*(?<FlightNumber>[A-Z]{2}\d{1,4})", RegexOptions.Compiled)]
    private static partial Regex FlightNumberRegex();

    public DateOnly FlightDate()
    {
        try
        {
            return DateOnly.ParseExact(Page.GetWords().SkipUntil(x => x.Text.Equals("Date:")).First().Text, "ddMMMyy");
        }
        catch (Exception e)
        {
            throw new MissingFlightDateException(e);
        }
    }
}