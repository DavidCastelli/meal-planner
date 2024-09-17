using Api.Common.Interfaces;

namespace Api.Common;

/// <summary>
/// A class implementing <see cref="IImageProcessingInfo"/>.
/// </summary>
/// <remarks>
/// The implementation of this class relies on the properties values to have already been configured
/// within the application configuration.
/// </remarks>
public sealed class ImageProcessingInfo : IImageProcessingInfo
{
    /// <inheritdoc/>
    public long ImageSizeLimit { get; }
    
    /// <inheritdoc/>
    public string[] PermittedExtensions { get; }
    
    /// <inheritdoc/>
    public string TempImageStoragePath { get; }
    
    /// <inheritdoc/>
    public string ImageStoragePath { get; }

    /// <summary>
    /// Creates a <see cref="ImageProcessingInfo"/>.
    /// </summary>
    /// <param name="config">The application configuration.</param>
    /// <exception cref="InvalidOperationException">Is thrown if any of the properties are not configured.</exception>
    public ImageProcessingInfo(IConfiguration config)
    {
        ImageSizeLimit = config.GetValue<long?>("ImageSizeLimit") ?? throw new InvalidOperationException("ImageSizeLimit is not configured.");
        PermittedExtensions = config.GetSection("PermittedExtensions").Get<string[]>() ?? throw new InvalidOperationException("PermittedExtensions is not configured.");
        TempImageStoragePath = config["TempImageStoragePath"] ?? throw new InvalidOperationException("TempImageStoragePath is not configured.");
        ImageStoragePath = config["ImageStoragePath"] ?? throw new InvalidOperationException("ImageStoragePath is not configured.");
    }
}