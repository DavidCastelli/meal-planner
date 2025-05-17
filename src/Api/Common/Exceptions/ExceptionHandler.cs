using System.Net.Mime;
using System.Text.Json;

using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Api.Common.Exceptions;

/// <summary>
/// Global exception handler used to map custom exceptions to the corresponding status codes and error messages before being sent back to the client.
/// </summary>
internal sealed class ExceptionHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        var problemDetails = exception switch
        {
            BadRequestException br => new ProblemDetails
            {
                Title = "BadRequest",
                Detail = br.Message,
                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
                Status = StatusCodes.Status400BadRequest,
                Extensions = new Dictionary<string, object?>
                {
                    { "errors", br.ToDictionary() }
                }
            },
            UnauthorizedException ua => new ProblemDetails
            {
                Title = "Unauthorized",
                Detail = ua.Message,
                Status = StatusCodes.Status401Unauthorized
            },
            ForbiddenException fe => new ProblemDetails
            {
                Title = "Forbidden",
                Detail = fe.Message,
                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.3",
                Status = StatusCodes.Status403Forbidden
            },
            NotFoundException nf => new ProblemDetails
            {
                Title = "NotFound",
                Detail = nf.Message,
                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.4",
                Status = StatusCodes.Status404NotFound
            },
            ConflictException ce => new ProblemDetails
            {
                Title = "Conflict",
                Detail = ce.Message,
                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.8",
                Status = StatusCodes.Status409Conflict
            },
            _ => new ProblemDetails
            {
                Title = "Server error",
                Type = "https://tools.ietf.org/html/rfc7231#section-6.6.1",
                Status = StatusCodes.Status500InternalServerError
            }
        };

        httpContext.Response.StatusCode = problemDetails.Status!.Value;

        await httpContext.Response.WriteAsJsonAsync(problemDetails, JsonSerializerOptions.Default,
            MediaTypeNames.Application.ProblemJson, cancellationToken);

        return true;
    }
}