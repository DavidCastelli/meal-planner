using Api.Domain.Meals;

namespace Api.Domain.Tags;

public sealed class Tag
{
    public int Id { get; set; }
    public TagType Type { get; set; }
    public ICollection<Meal> Meals { get; } = new List<Meal>();
}