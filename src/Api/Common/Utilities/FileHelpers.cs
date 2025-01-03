using System.Net;

namespace Api.Common.Utilities;

/// <summary>
/// Utility class used to help process files.
/// </summary>
public static class FileHelpers
{
    private static readonly Dictionary<string, List<byte[]>> FileSignature = new()
    {
        {
            ".jpeg",
            [
                new byte[] { 0xFF, 0xD8, 0xFF, 0xE0 },
                new byte[] { 0xFF, 0xD8, 0xFF, 0xE2 },
                new byte[] { 0xFF, 0xD8, 0xFF, 0xE3 }
            ]
        },
        {
            ".jpg",
            [
                new byte[] { 0xFF, 0xD8, 0xFF, 0xE0 },
                new byte[] { 0xFF, 0xD8, 0xFF, 0xE1 },
                new byte[] { 0xFF, 0xD8, 0xFF, 0xE8 }
            ]
        }
    };

    /// <summary>
    /// Processes a file by validating its correctness and writing it to the target <paramref name="storagePath"/>.
    /// </summary>
    /// <remarks>
    /// The file validation checks against file name length, empty files, files larger than the maximum allowed size, files whose file extension is not permitted,
    /// and files whose signature does not match the file extension. originalStoragePath should be provided when updating an existing file.
    /// When provided, the original file is deleted and replaced by the new file at the given storagePath.
    /// </remarks>
    /// <param name="formFile">The file to process.</param>
    /// <param name="tempStoragePath">The path to temporarily store the file before validation is complete.</param>
    /// <param name="storagePath">The path to store the file.</param>
    /// <param name="permittedExtensions">The permitted file extensions.</param>
    /// <param name="sizeLimit">The maximum allowed file size.</param>
    /// <param name="originalStoragePath">The original storage to be replaced if the image is updated.</param>
    /// <returns>
    /// A task which represents the asynchronous write operation.
    /// The result of the task upon completion returns a list of <see cref="Error"/> messages on failure,
    /// or an empty list on success.
    /// </returns>
    public static async Task<Error[]> ProcessFormFileAsync(IFormFile formFile, string tempStoragePath, string storagePath, string[] permittedExtensions, long sizeLimit, string? originalStoragePath = null)
    {
        List<Error> errors = [];

        var trustedFileNameForDisplay = WebUtility.HtmlEncode(formFile.FileName);

        if (trustedFileNameForDisplay.Length > FileErrors.MaxFileNameLength)
        {
            errors.Add(FileErrors.MaxFileName(trustedFileNameForDisplay));

            return [.. errors];
        }

        if (formFile.Length == 0)
        {
            errors.Add(FileErrors.Empty(trustedFileNameForDisplay));

            return [.. errors];
        }

        if (formFile.Length > sizeLimit)
        {
            var megabyteSizeLimit = sizeLimit / 1048576;
            errors.Add(FileErrors.ExceededMaximumSize(trustedFileNameForDisplay, megabyteSizeLimit));

            return [.. errors];
        }

        var isCreated = false;
        try
        {
            await using var fileStream = File.Create(tempStoragePath);
            await formFile.CopyToAsync(fileStream);
            isCreated = true;

            if (fileStream.Length == 0)
            {
                errors.Add(FileErrors.Empty(trustedFileNameForDisplay));
            }

            if (!IsValidFileExtensionAndSignature(formFile.FileName, fileStream, permittedExtensions))
            {
                errors.Add(FileErrors.InvalidExtensionOrSignature(trustedFileNameForDisplay));
            }
            else
            {
                if (originalStoragePath != null && File.Exists(originalStoragePath))
                {
                    File.Delete(originalStoragePath);
                }
                File.Move(tempStoragePath, storagePath);
                return [];
            }
        }
        catch (Exception ex)
        {
            errors.Add(FileErrors.UploadFailed(trustedFileNameForDisplay, ex.HResult));
        }

        if (isCreated)
        {
            File.Delete(tempStoragePath);
        }

        return [.. errors];
    }

    private static bool IsValidFileExtensionAndSignature(string fileName, Stream data, string[] permittedExtensions)
    {
        if (string.IsNullOrEmpty(fileName) || data.Length == 0)
        {
            return false;
        }

        var ext = Path.GetExtension(fileName).ToLowerInvariant();

        if (string.IsNullOrEmpty(ext) || !permittedExtensions.Contains(ext))
        {
            return false;
        }

        data.Position = 0;

        using var reader = new BinaryReader(data);
        var signatures = FileSignature[ext];
        var headerBytes = reader.ReadBytes(signatures.Max(m => m.Length));

        return signatures.Any(signature =>
            headerBytes.Take(signature.Length).SequenceEqual(signature));
    }
}