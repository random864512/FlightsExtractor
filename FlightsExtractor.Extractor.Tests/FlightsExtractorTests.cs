using FluentAssertions;

namespace FlightsExtractor.Extractor.Tests;

[TestFixture]
public class FlightsExtractorTests
{
    [Test]
    public async Task ShouldExtractFlightsData()
    {
        // Arrange
        var sut = new FlightsExtractor();
        var file = new FileInfo("SampleFile.pdf");

        // Act && Assert
        await new Func<Task>(async () => await sut.ExtractAsync(new FileInfo("SampleFile.pdf")))
            .Should()
            .ThrowAsync<NotImplementedException>();
    }
}
