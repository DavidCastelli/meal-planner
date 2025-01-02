namespace Api.Domain;

/// <summary>
/// Classes implementing this interface can be authorized.
/// </summary>
public interface IAuthorizable
{
    /// <summary>
    /// Gets the application user id.
    /// </summary>
    /// <value>
    /// An integer representing the id of a user.
    /// </value>
    public int ApplicationUserId { get; }
}