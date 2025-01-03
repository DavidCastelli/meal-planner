namespace Api.Common.Utilities;

/// <summary>
/// Utility class used to retrieve <see cref="Error"/> messages in relation to a tag.
/// </summary>
public static class TagErrors
{
    /// <summary>
    /// Error message that is used when a tag could not be found when searched for in the data store.
    /// </summary>
    /// <returns>An <see cref="Error"/> which contains an error code and description.</returns>
    public static Error NotFound() => new(
        "Tags.NotFound", "One or more tags do not exist.");
}