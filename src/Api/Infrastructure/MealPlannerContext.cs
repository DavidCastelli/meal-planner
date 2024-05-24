using Api.Domain.Todos;

using Microsoft.EntityFrameworkCore;

namespace Api.Infrastructure;

public sealed class MealPlannerContext : DbContext
{
    public MealPlannerContext(DbContextOptions<MealPlannerContext> options)
        : base(options)
    {
    }

    public DbSet<TodoItem> TodoItems => Set<TodoItem>();
}