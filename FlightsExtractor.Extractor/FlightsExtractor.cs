namespace FlightsExtractor.Extractor;

public interface IFlightsExtractor
{
    Task<Document> ExtractAsync(FileInfo file);
}

public class FlightsExtractor : IFlightsExtractor
{
    public async Task<Document> ExtractAsync(FileInfo file)
    {
        await Task.CompletedTask;
        throw new NotImplementedException();
    }
}
