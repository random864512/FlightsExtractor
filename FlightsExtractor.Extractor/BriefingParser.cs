using System.Text.RegularExpressions;
using UglyToad.PdfPig.Content;

namespace FlightsExtractor.Extractor;

internal partial class BriefingParser
{
    public BriefingPage? Parse(Page page)
    {
        if (FlightCrewBriefingRegex().IsMatch(string.Join(" ", page.GetWords())) && FirstPageRegex().IsMatch(string.Join(" ", page.GetWords())))
            return new BriefingPage();

        return default;
    }

    [GeneratedRegex(@"Page\s+1\s", RegexOptions.Compiled)]
    private static partial Regex FirstPageRegex();

    [GeneratedRegex(@"Flight\s+Crew\s+Briefing", RegexOptions.Compiled)]
    private static partial Regex FlightCrewBriefingRegex();
}
