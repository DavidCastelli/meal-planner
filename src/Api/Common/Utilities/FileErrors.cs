using Api.Domain;

namespace Api.Common.Utilities;

/// <summary>
/// Utility class used to retrieve <see cref="Error"/> messages as a result of unsuccessful file processing.
/// </summary>
public static class FileErrors
{
    /// <summary>
    /// Error message that is used when a file is empty.
    /// </summary>
    /// <param name="trustedFileNameForDisplay">The html encoded file name.</param>
    /// <returns>An <see cref="Error"/> which contains an error code and description.</returns>
    public static Error Empty(string trustedFileNameForDisplay) => new(
        "File.Empty", $"{trustedFileNameForDisplay} is empty.");

    /// <summary>
    /// Error message that is used when the file exceeds the maximum file size.
    /// </summary>
    /// <param name="trustedFileNameForDisplay">The html encoded file name.</param>
    /// <param name="megabyteSizeLimit">The maximum allowed file size in megabytes.</param>
    /// <returns>An <see cref="Error"/> which contains an error code and description.</returns>
    public static Error ExceededMaximumSize(string trustedFileNameForDisplay, long megabyteSizeLimit) => new(
        "File.ExceededMaximumSize", $"{trustedFileNameForDisplay} exceeds " + $"{megabyteSizeLimit} MB.");

    /// <summary>
    /// Error message that is used when a file does not have a permitted file extension
    /// or whose file signature does not match the extension. 
    /// </summary>
    /// <param name="trustedFileNameForDisplay">The html encoded file name.</param>
    /// <returns>An <see cref="Error"/> which contains an error code and description.</returns>
    public static Error InvalidExtensionOrSignature(string trustedFileNameForDisplay) => new(
        "File.InvalidExtensionOrSignature", $"${trustedFileNameForDisplay} file " +
                        "type isn't permitted or the file's signature " +
                        "doesnt match the file's extension.");

    /// <summary>
    /// Error message that is used when a file upload failed.
    /// </summary>
    /// <param name="trustedFileNameForDisplay">The html encoded file name.</param>
    /// <param name="hResult">The HResult of the exception causing the upload to fail.</param>
    /// <returns>An <see cref="Error"/> which contains an error code and description.</returns>
    public static Error UploadFailed(string trustedFileNameForDisplay, int hResult) => new(
        "File.UploadFailed", $"{trustedFileNameForDisplay} upload failed. " +
                             $"Error: {hResult}");
}