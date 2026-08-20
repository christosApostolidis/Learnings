namespace UserManagementAPI.Middleware;

public class TokenAuthenticationMiddleware
{
    private readonly RequestDelegate _next;
    private const string ValidToken = "my-token";

    public TokenAuthenticationMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Keep the welcome endpoint public
        if (context.Request.Path == "/")
        {
            await _next(context);
            return;
        }

        var authHeader = context.Request.Headers.Authorization.ToString();

        if (string.IsNullOrWhiteSpace(authHeader))
        {
            await WriteUnauthorized(context);
            return;
        }

        if (!authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
        {
            await WriteUnauthorized(context);
            return;
        }

        var token = authHeader["Bearer ".Length..].Trim();

        if (!string.Equals(token, ValidToken, StringComparison.Ordinal))
        {
            await WriteUnauthorized(context);
            return;
        }

        await _next(context);
    }

    private static async Task WriteUnauthorized(HttpContext context)
    {
        context.Response.StatusCode = 401;
        await context.Response.WriteAsJsonAsync(new
        {
            error = "Unauthorized."
        });
    }
}