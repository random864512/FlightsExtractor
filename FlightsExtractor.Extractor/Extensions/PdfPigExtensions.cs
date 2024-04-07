using MoreLinq;
using UglyToad.PdfPig.Content;

namespace FlightsExtractor.Extractor;

public static class PdfPigExtensions
{
    public static Result<string> GetWordAfter(this Page page, string word)
    {
        var wordsAfter = page.GetWords().SkipUntil(x => x.Text.Equals(word));
        var result = wordsAfter.FirstOrDefault()?.Text;
        if (result == default)
            return Error<string>($"word {word} not found");

        return Ok(result);
    }
}
