using Api.Domain.Todos;
using Api.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace Api.Features.TodoItems;

[ApiController]
[Route("[controller]")]
public class TodoItemsController : ControllerBase
{
    private readonly MealPlannerContext _context;

    public TodoItemsController(MealPlannerContext context)
    {
        _context = context;
    }

    [HttpPost("/api/todo-items")]
    public TodoItem Create(TodoItem todoItem)
    {
        _context.TodoItems.Add(todoItem);
        _context.SaveChanges();

        return todoItem;
    }
    
    [HttpGet("/api/todo-items")]
    public IEnumerable<TodoItem> GetAll()
    {
        return _context.TodoItems
            .AsNoTracking()
            .ToList();
    }
}