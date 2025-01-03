namespace Api.Common.Exceptions;

/// <summary>
/// Exception which is thrown when an image could not be found.
/// </summary>
public sealed class ImageNotFoundException : NotFoundException
{
    /// <summary>
    /// Creates a <see cref="ImageNotFoundException"/> given a <paramref name="fileName"/>.
    /// </summary>
    /// <param name="fileName">The file name of the image.</param>
    public ImageNotFoundException(string fileName)
        : base($"Image with filename {fileName} was not found.")
    {
    }
}