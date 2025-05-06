namespace Api.Common.Exceptions;

/// <summary>
/// Exception which is thrown when a shopping item could not be found when searched in the data store.
/// </summary>
public sealed class ShoppingItemNotFoundException : NotFoundException
{
    /// <summary>
    /// Creates a <see cref="ShoppingItemNotFoundException"/> given a <paramref name="shoppingItemId"/>.
    /// </summary>
    /// <param name="shoppingItemId">The id of the shopping item that could not be found.</param>
    public ShoppingItemNotFoundException(int shoppingItemId)
        : base($"Shopping item with id: {shoppingItemId} was not found")
    { }
}