using UserManagementAPI.Middleware;
using UserManagementAPI.Models;
using UserManagementAPI.Services;

var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();

app.UseMiddleware<ErrorHandlingMiddleware>();
app.UseMiddleware<TokenAuthenticationMiddleware>();
app.UseMiddleware<RequestLoggingMiddleware>();

var userService = new UserService();

app.MapGet("/", () => "User Management API is running!");

app.MapGet("/users", () => Results.Ok(userService.GetAll()));

app.MapGet("/users/{id:int}", IResult (int id) =>
{
    var user = userService.GetById(id);
    return user is null
        ? Results.NotFound("User not found.")
        : Results.Ok(user);
});

app.MapPost("/users", IResult (User user) =>
{
    try
    {
        var createdUser = userService.Create(user);
        return Results.Created($"/users/{createdUser.Id}", createdUser);
    }
    catch (ArgumentException ex)
    {
        return Results.BadRequest(ex.Message);
    }
    catch (InvalidOperationException ex)
    {
        return Results.Conflict(ex.Message);
    }
});

app.MapPut("/users/{id:int}", IResult (int id, User updatedUser) =>
{
    try
    {
        var user = userService.Update(id, updatedUser);
        return user is null
            ? Results.NotFound("User not found.")
            : Results.Ok(user);
    }
    catch (ArgumentException ex)
    {
        return Results.BadRequest(ex.Message);
    }
    catch (InvalidOperationException ex)
    {
        return Results.Conflict(ex.Message);
    }
});

app.MapDelete("/users/{id:int}", IResult (int id) =>
{
    if (!userService.Delete(id))
    {
        return Results.NotFound("User not found.");
    }

    return Results.NoContent();
});

app.Run();