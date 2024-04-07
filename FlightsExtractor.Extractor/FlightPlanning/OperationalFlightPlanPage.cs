using System.Text.RegularExpressions;
using UglyToad.PdfPig.Content;

namespace FlightsExtractor.Extractor;

public partial class OperationalFlightPlanPage(Page Page)
{
    public static bool IsOperationalFlightPlanPage(Page page) => page.Text.Contains("Operational Flight Plan");


    public string FlightNumber() => FlightNumberRegex().Match(Page.Text).Groups["FlightNumber"].Value;

    [GeneratedRegex(@"FltNr:\s*(?<FlightNumber>[A-Z]{2}\d{1,4})", RegexOptions.Compiled)]
    private static partial Regex FlightNumberRegex();

}