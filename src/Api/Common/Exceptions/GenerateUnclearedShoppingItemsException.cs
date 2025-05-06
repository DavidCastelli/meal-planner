namespace Api.Common.Exceptions;

/// <summary>
/// Exception that is thrown when a user attempts to generate shopping items before having cleared existing generated shopping items.
/// </summary>
public class GenerateUnclearedShoppingItemsException : ConflictException
{
    /// <summary>
    /// Creates a <see cref="GenerateUnclearedShoppingItemsException"/>.
    /// </summary>
    public GenerateUnclearedShoppingItemsException()
        : base("Shopping items already generated. Existing generated shopping items must be cleared before new ones can be generated.")
    { }
}