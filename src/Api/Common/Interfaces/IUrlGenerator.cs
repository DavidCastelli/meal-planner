namespace Api.Common.Interfaces;

/// <summary>
/// Classes implementing this interface are able to generate an url.
/// </summary>
public interface IUrlGenerator
{
    /// <summary>
    /// Generates a valid URL for an endpoint.
    /// </summary>
    /// <param name="endpointName">Name of the endpoint from which to generate a URL.</param>
    /// <param name="routeValues">Route values within the url.</param>
    /// <returns>A string representing a valid URL.</returns>
    public string GenerateUrl(string endpointName, object? routeValues);
}