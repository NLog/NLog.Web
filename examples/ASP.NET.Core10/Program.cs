using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http.HttpResults;
using NLog.Web;

var builder = WebApplication.CreateSlimBuilder(args);

// Setup NLog for Logging
await using var nlogFactory = NLog.LogManager.LogFactory;   // async flush/shutdown on dispose
builder.Logging.ClearProviders();
builder.UseNLog();

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.TypeInfoResolverChain.Insert(0, AppJsonSerializerContext.Default);
});

var app = builder.Build();

Todo[] sampleTodos =
[
    new(1, "Walk the dog"),
    new(2, "Do the dishes", DateOnly.FromDateTime(DateTime.Now)),
    new(3, "Do the laundry", DateOnly.FromDateTime(DateTime.Now.AddDays(1))),
    new(4, "Clean the bathroom"),
    new(5, "Clean the car", DateOnly.FromDateTime(DateTime.Now.AddDays(2)))
];

var todosApi = app.MapGroup("/todos");
todosApi.MapGet("/", (ILogger<Todo> logger) =>
{
    logger.LogInformation("GET Todos {TodoListCount} items", sampleTodos.Length);
    return sampleTodos;
});

todosApi.MapGet("/{id}", Results<Ok<Todo>, NotFound> (int id) =>
    sampleTodos.FirstOrDefault(a => a.Id == id) is { } todo
        ? TypedResults.Ok(todo)
        : TypedResults.NotFound());

app.Run();

public record Todo(int Id, string? Title, DateOnly? DueBy = null, bool IsComplete = false);

[JsonSerializable(typeof(Todo[]))]
internal partial class AppJsonSerializerContext : JsonSerializerContext
{

}
