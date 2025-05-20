using System.Net.Mime;

using Api.Common;
using Api.Domain.Tags;
using Api.Infrastructure;

using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Features.Tags;

/// <summary>
/// Controller that handles requests to get all tags.
/// </summary>
public sealed class GetTagsController : ApiControllerBase
{

    /// <summary>
    /// Action method responsible for getting all tags.
    /// </summary>
    /// <param name="handler">The handler for the request.</param>
    /// <param name="cancellationToken">The cancellation token for the request.</param>
    /// <returns>
    /// A task which represents the asynchronous read operation.
    /// The result of the task upon completion returns a <see cref="Results{TResult1, TResult2}"/> object.
    /// </returns>
    [HttpGet("/api/tags")]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized, MediaTypeNames.Application.ProblemJson)]
    [ProducesResponseType(typeof(IEnumerable<GetTagsDto>), StatusCodes.Status200OK, MediaTypeNames.Application.Json)]
    [Tags("Tags")]
    public async Task<Results<UnauthorizedHttpResult, Ok<IEnumerable<GetTagsDto>>>> GetAsync(GetTagsHandler handler, CancellationToken cancellationToken)
    {
        var getTagsDtos = await handler.HandleAsync(cancellationToken);

        return TypedResults.Ok(getTagsDtos);
    }
}

/// <summary>
/// The handler that handles getting all tags.
/// </summary>
public sealed class GetTagsHandler
{

    private readonly MealPlannerContext _dbContext;

    /// <summary>
    /// Creates a <see cref="GetTagsHandler"/>.
    /// </summary>
    /// <param name="dbContext">The database context.</param>
    public GetTagsHandler(MealPlannerContext dbContext)
    {
        _dbContext = dbContext;
    }

    /// <summary>
    /// Handles the database operations necessary to get all tags.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token of the request.</param>
    /// <returns>
    /// A task which represents the asynchronous read operation.
    /// The result of the task upon completion returns an enumerable <see cref="GetTagsDto"/>.
    /// </returns>
    public async Task<IEnumerable<GetTagsDto>> HandleAsync(CancellationToken cancellationToken)
    {
        var tags = await _dbContext.Tag
            .Select(t => new GetTagsDto(t.Id, t.Type))
            .ToListAsync(cancellationToken);

        return tags;
    }
}

/// <summary>
/// The DTO to return to the client after getting all tags.
/// </summary>
/// <param name="Id">The id of the tag.</param>
/// <param name="TagType">The type of the tag.</param>
public sealed record GetTagsDto(int Id, TagType TagType);