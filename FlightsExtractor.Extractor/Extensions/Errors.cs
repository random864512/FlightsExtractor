namespace FlightsExtractor.Extractor;

public static class Errors
{
    public static string MissingField(string field) => $"Missing field {field}";
    public static string InvalidFormat(string field) => $"Invalid format {field}";
}
