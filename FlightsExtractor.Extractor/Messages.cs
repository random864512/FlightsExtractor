using Microsoft.Extensions.Localization;

namespace FlightsExtractor.Extractor;

public class Messages(IStringLocalizer<Messages>? stringLocalizer = default)
{
    private string Localize(string text, params object[] parameters) => stringLocalizer != default ? (string)stringLocalizer[text, parameters] : string.Format(text, parameters);
    public string CannotParse(string field) => Localize("Cannot parse {0}", field);
    public string IncorrectDocumentStructure() => Localize("Incorrect document structure");
    public string IncorrectFormat(string field, string currentValue, string expectedFormat) =>
        Localize("{0} incorrect format, find {1}, expected {2}", field, currentValue, expectedFormat);
    public string IncorrectFormat(string field, string currentValue) => Localize("{0} incorrect format, find {1}", field, currentValue);
}
