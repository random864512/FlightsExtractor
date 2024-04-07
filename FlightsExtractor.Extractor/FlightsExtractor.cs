using System.Collections.Immutable;
using System.Text.RegularExpressions;
using UglyToad.PdfPig;

namespace FlightsExtractor.Extractor;

public interface IFlightsExtractor
{
    Document Extract(FileInfo file);
}

public partial class FlightsExtractor : IFlightsExtractor
{
    public Document Extract(FileInfo file)
    {
        using var pdf = PdfDocument.Open(file.FullName);
        var pages = pdf.GetPages();

        var operationalFlightPlans = pages.Where(x => x.Text.Contains("Operational Flight Plan"));

        return new Document(operationalFlightPlans.Select(plan => new Flight(new OperationalFlightPlan(new FlightNumber(MyRegex().Match(plan.Text).Groups["FlightNumber"].Value)))).ToImmutableList());
    }


    [GeneratedRegex(@"FltNr:\s*(?<FlightNumber>[A-Z]{2}\d{1,4})", RegexOptions.Compiled)]
    private static partial Regex MyRegex();
}
