using System.Net.Mime;

using Api.Common;
using Api.Common.Exceptions;
using Api.Common.Interfaces;
using Api.Domain;
using Api.Infrastructure;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Features.Images;

/// <summary>
/// Controller that handles requests to get an image by file name.
/// </summary>
public sealed class GetImageByFileNameController : ApiControllerBase
{
    /// <summary>
    /// Action method responsible for getting an image by file name.
    /// </summary>
    /// <param name="fileName">The file name of the image to get.</param>
    /// <param name="handler">The handler for the request</param>
    /// <param name="cancellationToken">The cancellation token for the request.</param>
    /// <returns>
    /// A task which represents the asynchronous read operation.
    /// The result of the task upon completion returns a <see cref="Results{TResult1, TResult2, TResult3, TResult4}"/> object.
    /// </returns>
    [HttpGet("/api/images/{fileName}", Name = "GetImageByFileName")]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized, MediaTypeNames.Application.ProblemJson)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound, MediaTypeNames.Application.ProblemJson)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden, MediaTypeNames.Application.ProblemJson)]
    [ProducesResponseType(typeof(PhysicalFileHttpResult), StatusCodes.Status200OK, MediaTypeNames.Image.Jpeg)]
    [Tags("Images")]
    public async Task<Results<UnauthorizedHttpResult, NotFound, ForbidHttpResult, PhysicalFileHttpResult>> GetByFileNameAsync(string fileName, GetImageByFileNameHandler handler, CancellationToken cancellationToken)
    {
        var imagePath = await handler.HandleAsync(fileName, cancellationToken);

        return TypedResults.PhysicalFile(imagePath, MediaTypeNames.Image.Jpeg);
    }
}

/// <summary>
/// The handler that handles getting the images path.
/// </summary>
public sealed class GetImageByFileNameHandler
{
    private readonly MealPlannerContext _dbContext;
    private readonly IUserContext _userContext;
    private readonly IAuthorizationService _authorizationService;

    /// <summary>
    /// Creates a <see cref="GetImageByFileNameHandler"/>.
    /// </summary>
    /// <param name="dbContext">The database context.</param>
    /// <param name="userContext">The user context.</param>
    /// <param name="authorizationService">The authorization service.</param>
    public GetImageByFileNameHandler(MealPlannerContext dbContext, IUserContext userContext,
        IAuthorizationService authorizationService)
    {
        _dbContext = dbContext;
        _userContext = userContext;
        _authorizationService = authorizationService;
    }

    /// <summary>
    /// Handles the database operation necessary to get the image path of an image by filename.
    /// </summary>
    /// <param name="fileName">The file name of the image whose image path to get.</param>
    /// <param name="cancellationToken">The cancellation token for the request.</param>
    /// <returns>
    /// A task which represents the asynchronous read operation.
    /// The result of the task upon completion returns the image path of the image.
    /// </returns>
    /// <exception cref="ImageNotFoundException">Is thrown if the image could not be found in the data store.</exception>
    /// <exception cref="ForbiddenException">Is thrown if the user is authenticated but lacks permission to access the resource.</exception>
    /// <exception cref="UnauthorizedException">Is thrown if the user lacks the necessary authentication credentials.</exception>
    public async Task<string> HandleAsync(string fileName, CancellationToken cancellationToken)
    {
        var image = await _dbContext.Image
            .Where(i => i.StorageFileName == fileName)
            .Select(i => new GetImageByFileNameDto(i.ImagePath, i.ManageableEntity.ApplicationUserId))
            .SingleOrDefaultAsync(cancellationToken);

        if (image == null)
        {
            throw new ImageNotFoundException(fileName);
        }

        var authorizationResult = await _authorizationService.AuthorizeAsync(_userContext.User, image, "SameUserPolicy");

        if (authorizationResult.Succeeded)
        {
            return image.ImagePath;
        }
        else if (_userContext.IsAuthenticated)
        {
            throw new ForbiddenException();
        }
        else
        {
            throw new UnauthorizedException();
        }
    }
}

/// <summary>
/// The DTO for the projected image data when getting an image by file name.
/// </summary>
/// <param name="ImagePath">The image path of the image.</param>
/// <param name="ApplicationUserId">The application user id of the user who the image belongs to.</param>
public sealed record GetImageByFileNameDto(string ImagePath, int ApplicationUserId) : IAuthorizable;