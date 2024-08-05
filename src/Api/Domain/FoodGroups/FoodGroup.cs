using Api.Domain.Ingredients;

namespace Api.Domain.FoodGroups;

public sealed class FoodGroup
{
    public int Id { get; set; }
    public FoodGroupType Type { get; set; }
    public ICollection<SubIngredient> Ingredients { get; } = new List<SubIngredient>();
}