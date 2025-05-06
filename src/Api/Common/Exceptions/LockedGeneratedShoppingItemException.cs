namespace Api.Common.Exceptions;

/// <summary>
/// Exception that is thrown when a user attempts to lock a shopping item that was generated.
/// </summary>
public class LockedGeneratedShoppingItemException : ConflictException
{
    /// <summary>
    /// Creates a <see cref="LockedGeneratedShoppingItemException"/>.
    /// </summary>
    /// <param name="shoppingItemId">The id of the shopping item attempted to be locked.</param>
    public LockedGeneratedShoppingItemException(int shoppingItemId)
        : base($"Shopping item with id: {shoppingItemId} could not be locked as it is currently marked as generated." +
               "Only user created shopping items can be locked.")
    { }
}