namespace Api.Common.Interfaces;

/// <summary>
/// Classes implementing this interface are able to retrieve information on the image processing configuration.
/// </summary>
public interface IImageProcessingInfo
{
    /// <summary>
    /// Gets the image size limit of the application.
    /// </summary>
    /// <value>
    /// A 64-bit integer specifying the maximum image size in bytes supported by the application.
    /// </value>
    public long ImageSizeLimit { get; }
    
    /// <summary>
    /// Gets the permitted file extensions of the application.
    /// </summary>
    /// <value>
    /// A string array of supported file extensions.
    /// </value>
    public string[] PermittedExtensions { get; }
    
    /// <summary>
    /// Gets the temporary image storage path of the application.
    /// </summary>
    /// <remarks>
    /// All images are initially stored in this location until validation is complete.
    /// Images are then moved to <see cref="ImageStoragePath"/>.
    /// </remarks>
    /// <value>
    /// The absolute path where images are temporarily stored.
    /// </value>
    public string TempImageStoragePath { get; }
    
    /// <summary>
    /// Gets the image storage path of the application.
    /// </summary>
    /// <value>
    /// The absolute path where images are stored.
    /// </value>
    public string ImageStoragePath { get; }
}