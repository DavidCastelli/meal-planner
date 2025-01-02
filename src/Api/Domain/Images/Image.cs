using Api.Domain.ManageableEntities;

namespace Api.Domain.Images;

/// <summary>
/// Represents image metadata.
/// </summary>
public sealed class Image
{
    /// <summary>
    /// Gets or sets the id of an image.
    /// </summary>
    /// <value>
    /// An integer specifying the id of an image.
    /// </value>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the storage file name of the image.
    /// </summary>
    /// <remarks>
    /// The storage file name is the file name that is used when storing the image on the server.
    /// The storage file name is randomly generated and uniquely identifies the image. 
    /// </remarks>
    /// <value>
    /// A string specifying the storage file name of the image.
    /// </value>
    public required string StorageFileName { get; set; }

    /// <summary>
    /// Gets or sets the display file name of the image.
    /// </summary>
    /// <remarks>
    /// The display file name is the file name provided by the client when the image is uploaded.
    /// The display file name is html encoded before being stored. 
    /// </remarks>
    /// <value>
    /// A string specifying the display file name of the image.
    /// </value>
    public required string DisplayFileName { get; set; }

    /// <summary>
    /// Gets or sets the image path of the image.
    /// </summary>
    /// <remarks>
    /// The image path is the location in which the image is stored on the server.
    /// The path consists of a storage location which is set in the application configuration
    /// and a file name which is randomly generated.
    /// </remarks>
    /// <value>
    /// A string specifying the path to the image.
    /// </value>
    public required string ImagePath { get; set; }

    /// <summary>
    /// Gets or sets the URL of the image.
    /// </summary>
    /// <remarks>
    /// The image URL is the location at which the client can get the image.
    /// </remarks>
    /// <value>
    /// A string specifying a valid URL for the image.
    /// </value>
    public required string ImageUrl { get; set; }

    /// <summary>
    /// Gets or sets the manageable entity id.
    /// </summary>
    /// <value>
    /// An integer representing the id of a manageable entity which the image belongs to.
    /// </value>
    public required int ManageableEntityId { get; set; }

    /// <summary>
    /// Gets or sets the manageable entity.
    /// </summary>
    /// <value>
    /// The manageable entity which the image belongs to.
    /// </value>
    public required ManageableEntity ManageableEntity { get; set; }
}