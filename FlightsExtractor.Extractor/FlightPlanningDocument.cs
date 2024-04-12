using Tabula;
using Tabula.Extractors;
using UglyToad.PdfPig;
using UglyToad.PdfPig.Content;
using UglyToad.PdfPig.DocumentLayoutAnalysis.TextExtractor;

namespace FlightsExtractor.Extractor;

/* Wrapper for pdfDocument */
internal class FlightPlanningDocument(
    PdfDocument document,
    ImmutableList<OperationalFlightPlanningPage> PlanningPages,
    ImmutableList<CrewBriefingPage> BriefingPages) : IDisposable
{
    public ImmutableList<OperationalFlightPlanningPage> PlanningPages { get; } = PlanningPages;
    public ImmutableList<CrewBriefingPage> BriefingPages { get; } = BriefingPages;

    public static FlightPlanningDocument Create(string filePath)
    {
        ImmutableList<OperationalFlightPlanningPage> PlanningPages = [];
        ImmutableList<CrewBriefingPage> BriefingPages = [];
        PdfDocument? document = default;

        bool success = false;
        try
        {
            document = PdfDocument.Open(filePath);
            var extractor = new ObjectExtractor(document);
            foreach (var page in document.GetPages())
            {
                var pdfPage = new PdfPage(extractor, page);
                var operationalFlightPlanningPage = OperationalFlightPlanningPage.Create(pdfPage);
                if (operationalFlightPlanningPage != null)
                    PlanningPages = [.. PlanningPages, operationalFlightPlanningPage];

                var briefingPage = CrewBriefingPage.Create(pdfPage);
                if (briefingPage != default)
                    BriefingPages = [.. BriefingPages, briefingPage];
            }

            success = true;
            return new FlightPlanningDocument(document, PlanningPages, BriefingPages);
        }
        finally
        {
            if (!success)
                document?.Dispose();
        }
    }

    public void Dispose() => document?.Dispose();
}

internal record PdfPage(ObjectExtractor Extractor, Page Page)
{
    public string Text() => LazyText.Value;
    private Lazy<string> LazyText { get; } = new(() => Page.Text);

    public string ContentOrderedText() => LazyContentOrderText.Value;

    private Lazy<string> LazyContentOrderText { get; } = new(() => ContentOrderTextExtractor.GetText(Page));

    public ImmutableList<Table> Tables() => LazyTables.Value;

    private Lazy<ImmutableList<Table>> LazyTables { get; } = new(() => new SpreadsheetExtractionAlgorithm().Extract(Extractor.Extract(Page.Number)).ToImmutableList());
}


