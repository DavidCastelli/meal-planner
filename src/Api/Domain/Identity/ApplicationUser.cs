using Api.Domain.Meals;
using Api.Domain.Recipes;

using Microsoft.AspNetCore.Identity;

namespace Api.Domain.Identity;

public class ApplicationUser : IdentityUser<int>
{
    public ICollection<Meal> Meals { get; } = new List<Meal>();
    public ICollection<Recipe> Recipes { get; } = new List<Recipe>();
}