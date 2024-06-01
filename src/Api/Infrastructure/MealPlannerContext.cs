using Api.Domain.Todos;
using Api.Features.Identity;

using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Api.Infrastructure;

public sealed class MealPlannerContext : IdentityDbContext<ApplicationUser>
{
    public MealPlannerContext(DbContextOptions<MealPlannerContext> options)
        : base(options)
    {
    }

    public DbSet<TodoItem> TodoItems => Set<TodoItem>();
}