namespace Api.Domain.ShoppingItems;

/// <summary>
/// Entity which represents a shopping item.
/// </summary>
/// <remarks>
/// Shopping items are generated based on the ingredients of all currently scheduled meals.
/// Additional shopping items may be created by the user.
/// </remarks>
public sealed class ShoppingItem : IAuthorizable
{
    /// <summary>
    /// Gets or sets the id of a shopping item.
    /// </summary>
    /// <value>
    /// An integer specifying the id of a shopping item.
    /// </value>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the name of a shopping item.
    /// </summary>
    /// <remarks>The name of a shopping item is obtained from an ingredient of a scheduled meal, or provided by the user.</remarks>
    /// <value>
    /// A string specifying the name of a shopping item.
    /// </value>
    public required string Name { get; set; }

    /// <summary>
    /// Gets or sets the measurement of a shopping item.
    /// </summary>
    /// <remarks>The measurement of a shopping item is obtained from an ingredient of a scheduled meal, or provided by the user.</remarks>
    /// <value>
    /// A string specifying the measurement of a shopping item.
    /// </value>
    public string? Measurement { get; set; }

    /// <summary>
    /// Gets or sets the price of a shopping item.
    /// </summary>
    /// <value>
    /// A decimal representing the price of a shopping item.
    /// </value>
    public decimal? Price { get; set; }

    /// <summary>
    /// Gets or sets the quantity of a shopping item.
    /// </summary>
    /// <value>
    /// An integer specifying the quantity of a shopping item.
    /// </value>
    public int? Quantity { get; set; }

    /// <summary>
    /// Gets or sets if a shopping item is checked.
    /// </summary>
    /// <remarks>A shopping item is checked by the user once they have acquired the shopping item.</remarks>
    /// <value>
    /// A bool specifying if the shopping item is checked.
    /// </value>
    public bool IsChecked { get; set; }

    /// <summary>
    /// Gets or sets if a shopping item is locked.
    /// </summary>
    /// <remarks>
    /// A shopping item is locked by the user so that it is kept if the shopping list is cleared.
    /// Only shopping items created by the user where IsGenerated are set to false may be locked.
    /// </remarks>
    /// <value>
    /// A bool specifying if the shopping item is locked.
    /// </value>
    public bool IsLocked { get; set; }

    /// <summary>
    /// Gets or sets if a shopping item is generated.
    /// </summary>
    /// <remarks>
    /// A shopping item is generated when it is created based on a currently scheduled ingredient.
    /// A shopping item is considered to not be generated if it is created manually by the user.
    /// </remarks>
    /// <value>
    /// A bool specifying if a shopping item is generated.
    /// </value>
    public bool IsGenerated { get; set; }

    /// <summary>
    /// Gets or sets the application user id.
    /// </summary>
    /// <value>
    /// An integer representing the id of a user who the shopping item entity belongs to.
    /// </value>
    public required int ApplicationUserId { get; set; }
}