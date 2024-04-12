using Tabula;

namespace FlightsExtractor.Extractor;

internal partial class CrewBriefingPage(PdfPage page)
{
    public static CrewBriefingPage? Create(PdfPage page)
    {
        if (page.Text().Contains("Flight Assignment / Flight Crew Briefing") && page.Text().Contains("1of4Page"))
            return new CrewBriefingPage(page);

        return default;
    }

    public (ImmutableList<(string? function, string? name)>? crewMembers, string? flightNumber)? ParseCrewTable()
    {
        foreach (var table in page.Tables())
        {
            if (table.Rows.Count == 0)
                continue;

            if (table.Rows[0].Select(x => x.GetText().Trim()).Intersect(["Func", "3LC", "Name"]).Count() != 3)
                continue;

            return (ParseCrewMembers(table).ToImmutableList(), ParseFlightNumber(table));
        }

        return default;

        static string? ParseFlightNumber(Table table) => table.Rows[0][3].TextElements[0].GetText().Trim();

        IEnumerable<(string? function, string? name)> ParseCrewMembers(Table table)
        {
            foreach (var row in table.Rows.Skip(1).Where(x => !x.All(x => string.IsNullOrWhiteSpace(x.GetText()))))
            {
                var function = row.Count >= 1 ? row[0].GetText().Trim() : default;
                var name = row.Count >= 3 ? Regex.Replace(row[2].GetText(), @"\s+", " ").Trim() : default;

                yield return (function, name);
            }
        }
    }
}
