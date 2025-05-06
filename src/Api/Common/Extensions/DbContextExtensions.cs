using Api.Common.Exceptions;
using Api.Common.Utilities;

using Microsoft.EntityFrameworkCore;

using Npgsql;

namespace Api.Common.Extensions;

/// <summary>
/// Extension methods for the <see cref="DbContext"/> class.
/// </summary>
internal static class DbContextExtensions
{
    /// <summary>
    /// Extension method used to save changes to the database and write an image to physical storage.
    /// </summary>
    /// <param name="dbContext">The database context.</param>
    /// <param name="image">The file to process.</param>
    /// <param name="tempFilePath">The path to temporarily store the file before validation is complete.</param>
    /// <param name="filePath">The path to store the file.</param>
    /// <param name="permittedExtensions">The permitted file extensions.</param>
    /// <param name="sizeLimit">The maximum allowed file size.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <param name="originalFilePath">The original storage to be replaced if the image is updated.</param>
    /// <returns>
    /// A task which represents the asynchronous write operation.
    /// </returns>
    /// <exception cref="ImageProcessingException">Is thrown if validation on the <paramref name="image"/> fails.</exception>
    /// <exception cref="UniqueConstraintViolationException">Is thrown if a unique constraint violation occurs.</exception>
    public static async Task SaveChangesSaveImageAsync(this DbContext dbContext, IFormFile image,
        string tempFilePath, string filePath, string[] permittedExtensions, long sizeLimit,
        CancellationToken cancellationToken, string? originalFilePath = null)
    {
        await using var transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);
        try
        {
            await dbContext.SaveChangesAsync(cancellationToken);

            var imageProcessingErrors = await FileHelpers.ProcessFormFileAsync(image, tempFilePath, filePath,
                permittedExtensions, sizeLimit, originalFilePath);

            if (imageProcessingErrors.Length != 0)
            {
                throw new ImageProcessingException(imageProcessingErrors);
            }

            await dbContext.Database.CommitTransactionAsync(CancellationToken.None);
        }
        catch (DbUpdateConcurrencyException)
        {
            await transaction.RollbackAsync(CancellationToken.None);
            throw;
        }
        catch (DbUpdateException ex)
        {
            if (ex.GetBaseException() is PostgresException
                {
                    SqlState: PostgresErrorCodes.UniqueViolation
                } postgresException)
            {
                await transaction.RollbackAsync(CancellationToken.None);

                var cause = postgresException.ConstraintName switch
                {
                    "IX_ManageableEntity_ApplicationUserId_Title" => "Title",
                    "IX_SubIngredient_RecipeId_Name" => "SubIngredient names",
                    "IX_Direction_RecipeId_Number" => "Direction numbers",
                    _ => null
                };

                throw new UniqueConstraintViolationException(cause);
            }

            await transaction.RollbackAsync(CancellationToken.None);
            throw;
        }
        catch (ImageProcessingException)
        {
            await transaction.RollbackAsync(CancellationToken.None);
            throw;
        }
        catch (OperationCanceledException)
        {
            await transaction.RollbackAsync(CancellationToken.None);
            throw;
        }
        catch (Exception)
        {
            await transaction.RollbackAsync(CancellationToken.None);
            throw;
        }
    }

    /// <summary>
    /// Extension method used to save changes to the database and delete an image from physical storage.
    /// </summary>
    /// <param name="dbContext">The database context.</param>
    /// <param name="filePath">The path to store the file.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>
    /// A task which represents the asynchronous write operation.
    /// </returns>
    /// <exception cref="UniqueConstraintViolationException">Is thrown if a unique constraint violation occurs.</exception>
    public static async Task SaveChangesDeleteImageAsync(this DbContext dbContext, string filePath, CancellationToken cancellationToken)
    {
        await using var transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);
        try
        {
            await dbContext.SaveChangesAsync(cancellationToken);

            File.Delete(filePath);

            await dbContext.Database.CommitTransactionAsync(CancellationToken.None);
        }
        catch (DbUpdateConcurrencyException)
        {
            await transaction.RollbackAsync(CancellationToken.None);
            throw;
        }
        catch (DbUpdateException ex)
        {
            if (ex.GetBaseException() is PostgresException
                {
                    SqlState: PostgresErrorCodes.UniqueViolation
                } postgresException)
            {
                await transaction.RollbackAsync(CancellationToken.None);

                var cause = postgresException.ConstraintName switch
                {
                    "IX_ManageableEntities_ApplicationUserId_Title" => "Title",
                    "IX_SubIngredient_RecipeId_Name" => "SubIngredient names",
                    "IX_Direction_RecipeId_Number" => "Direction numbers",
                    _ => null
                };

                throw new UniqueConstraintViolationException(cause);
            }

            await transaction.RollbackAsync(CancellationToken.None);
            throw;
        }
        catch (ImageProcessingException)
        {
            await transaction.RollbackAsync(CancellationToken.None);
            throw;
        }
        catch (OperationCanceledException)
        {
            await transaction.RollbackAsync(CancellationToken.None);
            throw;
        }
        catch (Exception)
        {
            await transaction.RollbackAsync(CancellationToken.None);
            throw;
        }
    }
}