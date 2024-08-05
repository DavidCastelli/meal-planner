namespace Api.Domain.Ingredients;

public sealed class Ingredient
{
    public required string Name { get; set; }
    public required string Measurement { get; set; }
}