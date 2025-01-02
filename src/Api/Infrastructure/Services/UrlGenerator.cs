using Api.Common.Interfaces;

namespace Api.Infrastructure.Services;

/// <summary>
/// A class implementing <see cref="IUrlGenerator"/>.
/// </summary>
/// <remarks>
/// The implementation of this class relies on the <see cref="IHttpContextAccessor"/> whose use can be dangerous.
/// The <see cref="UrlGenerator"/> should only be used in the scope of a request where the http context will be available.
/// </remarks>
public sealed class UrlGenerator : IUrlGenerator
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly LinkGenerator _linkGenerator;

    /// <summary>
    /// Creates a <see cref="UrlGenerator"/>.
    /// </summary>
    /// <param name="httpContextAccessor">The http context accessor.</param>
    /// <param name="linkGenerator">The link generator.</param>
    public UrlGenerator(IHttpContextAccessor httpContextAccessor, LinkGenerator linkGenerator)
    {
        _httpContextAccessor = httpContextAccessor;
        _linkGenerator = linkGenerator;
    }

    /// <inheritdoc/>
    /// <exception cref="InvalidOperationException">Is thrown if the http context is not available, or the URL generation failed.</exception>
    public string GenerateUrl(string endpointName, object? routeValues)
    {
        return _httpContextAccessor.HttpContext == null
            ? throw new InvalidOperationException("URL generator is unavailable")
            : _linkGenerator.GetUriByName(_httpContextAccessor.HttpContext, endpointName, routeValues)
            ?? throw new InvalidOperationException("Failed to generate URL");
    }
}