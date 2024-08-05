using Api.Domain.Identity;
using Api.Domain.Recipes;
using Api.Domain.Tags;

namespace Api.Domain.Meals;

public sealed class Meal
{
    public int Id { get; set; }
    public required string Title { get; set; }
    public string? Image { get; set; }
    public ICollection<Tag> Tags { get; } = new List<Tag>();
    public ICollection<Recipe> Recipes { get; } = new List<Recipe>();
    public int ApplicationUserId { get; set; }
    public required ApplicationUser User { get; set; }
    
}