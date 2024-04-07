namespace FlightsExtractor.Extractor;

public interface IFlightPlanningExtractor
{
    FlightPlanning Extract(FileInfo file);
}

public partial class FlightPlanningExtractor : IFlightPlanningExtractor
{
    public FlightPlanning Extract(FileInfo file)
    {
        using var flightsPlanningDocument = FlightPlanningDocument.Create(file);
        var flights = flightsPlanningDocument.GetFlights();

        return new FlightPlanning(flights.Select(flight =>
            new Flight(
                new OperationalFlightPlan(
                    new FlightNumber(flight.OperationalFlightPlan.FlightNumber())
                )
            )));
    }
}
