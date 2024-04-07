namespace FlightsExtractor.Extractor;

public record DocPage;

public record BriefingPage : DocPage;

internal record PlanPage(
    FlightNumber FlightNumber,
    DateOnly FlightDate
) : DocPage;
