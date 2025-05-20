using System.Net;

using Api.Common.Utilities;

using Microsoft.AspNetCore.Http;

namespace Api.UnitTests.Common.Utilities;

public class FileHelpersTests
{
    [Fact]
    public async Task ProcessFormFileAsync_ShouldReturnMaxFileNameError_WhenFileDisplayNameLengthIsMoreThan255Characters()
    {
        await using var stream = new MemoryStream();
        await using var writer = new StreamWriter(stream);
        stream.Position = 0;
        const string fileName = "lorem ipsum dolor sit amet dolor sit amet dolor sit amet dolor sit amet dolor sit" +
                                " amet dolor sit amet dolor sit amet dolor sit amet dolor sit amet dolor sit amet dolor sit" +
                                " amet dolor sit amet dolor sit amet dolor sit amet dolor sit amet dolor sit amet dolor sit.jpg";
        IFormFile file = new FormFile(stream, 0, stream.Length, "Test", fileName);

        var result = await FileHelpers.ProcessFormFileAsync(file, "", "", [], 0);

        Assert.Single(result);
        Assert.Equal(result[0], FileErrors.MaxFileName(WebUtility.HtmlEncode(fileName)));
    }

    [Fact]
    public async Task ProcessFormFileAsync_ShouldReturnEmptyError_WhenFileIsEmpty()
    {
        await using var stream = new MemoryStream();
        await using var writer = new StreamWriter(stream);
        stream.Position = 0;
        const string fileName = "Test.jpg";
        IFormFile file = new FormFile(stream, 0, 0, "Test", fileName);

        var result = await FileHelpers.ProcessFormFileAsync(file, "", "", [], 0);

        Assert.Single(result);
        Assert.Equal(result[0], FileErrors.Empty(WebUtility.HtmlEncode(fileName)));
    }

    [Fact]
    public async Task ProcessFormFileAsync_ShouldReturnExceededMaximumSizeError_WhenFileSizeGreaterThanFiveMegabytes()
    {
        await using var stream = new MemoryStream();
        await using var writer = new StreamWriter(stream);
        stream.Position = 0;
        const string fileName = "Test.jpg";
        IFormFile file = new FormFile(stream, 0, 5242881, "Test", fileName);

        var result = await FileHelpers.ProcessFormFileAsync(file, "", "", [], 5242880);

        Assert.Single(result);
        Assert.Equal(result[0], FileErrors.ExceededMaximumSize(WebUtility.HtmlEncode(fileName), 5242880 / 1048576));
    }
}