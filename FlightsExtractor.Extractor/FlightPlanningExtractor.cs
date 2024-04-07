using System.Collections.Immutable;
using UglyToad.PdfPig;

namespace FlightsExtractor.Extractor;

public interface IFlightPlanningExtractor
{
    FlightPlanning Extract(FileInfo file);
}

internal class FlightPlanningExtractor : IFlightPlanningExtractor
{
    private readonly BriefingParser crewBriefingParser;
    private readonly PlanParser operationalFlightPlanParser;

    public FlightPlanningExtractor()
    {
        crewBriefingParser = new BriefingParser();
        operationalFlightPlanParser = new PlanParser();
    }

    public FlightPlanning Extract(FileInfo file)
    {
        if (!file.Exists)
            throw new FileDoesNotExistException();

        try
        {
            using var pdf = PdfDocument.Open(file.FullName);
            var pages = pdf.GetPages().Select(page => crewBriefingParser.Parse(page) as DocPage ?? operationalFlightPlanParser.Parse(page)).Where(x => x is not null);

            if (pages.OfType<PlanPage>().Count() != pages.OfType<BriefingPage>().Count())
                throw new InvalidPdfStructureException("Missing operational flight plan or crew briefing");

            var flights = pages.OfType<PlanPage>().Zip(pages.OfType<BriefingPage>(), (plan, briefing) => (plan, briefing)).ToImmutableList();

            return new FlightPlanning(flights.Select(flight =>
                new Flight(
                    new OperationalFlightPlan(
                        flight.plan.FlightNumber,
                        flight.plan.FlightDate,
                        flight.plan.AircraftRegistration,
                        new Route(flight.plan.From, flight.plan.To)
                    )
                )));
        }
        catch (Exception e)
        {
            throw new InvalidPdfException("PDF cannot be opened, it may be corrupted or in incorrect format.", e);
        }
    }

}