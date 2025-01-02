using Api.Domain.Images;

namespace Api.Domain.ManageableEntities;

/// <summary>
/// Base class for a manageable entity.
/// </summary>
/// <remarks>
/// Classes inheriting from <see cref="ManageableEntity"/> are mapped using the TPT pattern.
/// </remarks>
public class ManageableEntity : IAuthorizable
{
    /// <summary>
    /// Gets or sets the id of a manageable entity.
    /// </summary>
    /// <value>
    /// An integer specifying the id of a manageable entity.
    /// </value>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the title of a manageable entity.
    /// </summary>
    /// <value>
    /// A string specifying the title of a manageable entity.
    /// </value>
    public required string Title { get; set; }

    /// <summary>
    /// Gets or sets the image of a manageable entity.
    /// </summary>
    /// <value>
    /// An <see cref="Domain.Images.Image"/> of a manageable entity.
    /// </value>
    public Image? Image { get; set; }

    /// <summary>
    /// Gets or sets the application user id.
    /// </summary>
    /// <value>
    /// An integer representing the id of a user who the manageable entity belongs to.
    /// </value>
    public required int ApplicationUserId { get; set; }
}