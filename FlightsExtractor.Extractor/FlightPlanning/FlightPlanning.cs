using System.Collections.Immutable;
using UglyToad.PdfPig;

namespace FlightsExtractor.Extractor;

internal sealed partial class FlightPlanningDocument(PdfDocument document) : IDisposable
{
    public ImmutableList<(OperationalFlightPlanPage OperationalFlightPlan, CrewBriefingPage crewBriefing)> GetFlights()
    {
        var operationalFlightPlanPages = document.GetPages().Where(OperationalFlightPlanPage.IsOperationalFlightPlanPage).Select(page => new OperationalFlightPlanPage(page)).ToImmutableList();
        var crewBriefingPages = document.GetPages().Where(CrewBriefingPage.IsCrewBriefingPage).Select(page => new CrewBriefingPage(page)).ToImmutableList();

        if (operationalFlightPlanPages.Count() != crewBriefingPages.Count())
            throw new MissingOperationalFlightPlanOrCrewBriefingException();

        return operationalFlightPlanPages.Zip(crewBriefingPages, (plan, briefing) => (plan, briefing)).ToImmutableList();
    }

    public static FlightPlanningDocument Create(FileInfo file)
    {
        if (!file.Exists)
            throw new FileDoesNotExistException();

        try
        {
            return new FlightPlanningDocument(PdfDocument.Open(file.FullName));
        }
        catch (Exception e)
        {
            throw new InvalidPdfException("PDF cannot be opened, it may be corrupted or in incorrect format.", e);
        }
    }

    public void Dispose() => document.Dispose();
}
