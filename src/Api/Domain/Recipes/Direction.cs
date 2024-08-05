using Api.Domain.Recipes;

namespace Api.Domain.Recipes;

public sealed class Direction
{
    public int Number { get; set; }
    public required string Description { get; set; }
}