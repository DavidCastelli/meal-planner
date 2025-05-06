namespace Api.Common.Exceptions;

/// <summary>
/// Exception that is thrown when a user attempts to delete a shopping item that is locked.
/// </summary>
public sealed class ShoppingItemLockedException : ConflictException
{
    /// <summary>
    /// Creates a <see cref="ShoppingItemNotFoundException"/>.
    /// </summary>
    /// <param name="shoppingItemId">The id of the shopping item attempted to be deleted.</param>
    public ShoppingItemLockedException(int shoppingItemId)
        : base($"Shopping item with id: {shoppingItemId} could not be delete as it is currently marked as locked." +
               "To delete the shopping item it must first be unlocked.")
    { }
}