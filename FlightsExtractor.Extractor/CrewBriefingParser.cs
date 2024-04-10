using System.Collections.Immutable;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;
using Tabula;
using Tabula.Extractors;
using UglyToad.PdfPig;

namespace FlightsExtractor.Extractor;

internal record CrewBriefingPage
(
    Result<FlightNumber> FlightNumber,
    ImmutableList<CrewMember> CrewMembers
);

internal partial class CrewBriefingParser(ILogger<CrewBriefingParser>? logger = default)
{
    public IEnumerable<CrewBriefingPage> Parse(PdfDocument document)
    {
        logger?.LogDebug("Parsing crew briefings");

        ObjectExtractor objectExtractor = new(document);
        SpreadsheetExtractionAlgorithm tableExtractor = new();

        foreach (var page in document.GetPages().Where(x => x.Text.Contains("Flight Assignment / Flight Crew Briefing") && x.Text.Contains("1of4Page")))
        {
            logger?.LogTrace("Parsing page {pageNumber}", page.Number);
            var pageArea = objectExtractor.Extract(page.Number);
            var tables = tableExtractor.Extract(pageArea);
            var crewTable = tables.Select(table => ParseCrewTable(table)).FirstOrDefault(x => x != default);
            if (crewTable != default)
                yield return new CrewBriefingPage(crewTable!.Value.flightNumber, crewTable!.Value.crewMembers);
        }
    }

    private static (ImmutableList<CrewMember> crewMembers, Result<FlightNumber> flightNumber)? ParseCrewTable(Table table)
    {
        if (table.Rows.Count == 0)
            return default;

        if (table.Rows[0].Select(x => x.GetText().Trim()).Intersect(["Func", "3LC", "Name"]).Count() != 3)
            return default;

        return (ParseCrewMembers(table).ToImmutableList(), ParseFlightNumber(table));

        static Result<FlightNumber> ParseFlightNumber(Table table) => FlightNumber.Create(table.Rows[0][3].TextElements[0].GetText().Trim());

        static IEnumerable<CrewMember> ParseCrewMembers(Table table)
        {
            foreach (var row in table.Rows.Skip(1).Where(x => !x.All(x => string.IsNullOrWhiteSpace(x.GetText()))))
            {
                var function = row.Count >= 1 ? row[0].GetText().Trim() : Error<string>(string.Empty);
                var name = row.Count >= 3 ? Regex.Replace(row[2].GetText(), @"\s+", " ").Trim()! : Error<string>(string.Empty);

                yield return new CrewMember(function, name);
            }
        }
    }
}
