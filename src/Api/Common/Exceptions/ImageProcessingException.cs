using Api.Domain;

namespace Api.Common.Exceptions;

/// <summary>
/// Exception which is thrown when image processing fails.
/// </summary>
public sealed class ImageProcessingException : BadRequestException
{
    /// <summary>
    /// Creates a <see cref="ImageProcessingException"/> given a list of <paramref name="errors"/>.
    /// </summary>
    /// <param name="errors">Array of <see cref="Error"/> messages for the client.</param>
    public ImageProcessingException(Error[] errors)
        : base("Image processing failed", errors)
    {
    }
}