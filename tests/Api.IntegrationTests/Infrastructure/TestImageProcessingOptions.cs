namespace Api.IntegrationTests.Infrastructure;

public class TestImageProcessingOptions
{
    public const string ImageProcessing = "ImageProcessing";

    public long ImageSizeLimit { get; set; }

    public string[] PermittedExtensions { get; set; } = [];

    public string TempImageStoragePath { get; set; } = String.Empty;

    public string ImageStoragePath { get; set; } = String.Empty;

    public string ImageAssetsPath { get; set; } = String.Empty;

    public string ImageBaseUrl { get; set; } = String.Empty;
}