namespace ReceptionBook.Middleware;

public class SecurityHeadersMiddleware
{
    private const string SwaggerPath = "/swagger";

    private readonly RequestDelegate _next;

    public SecurityHeadersMiddleware(RequestDelegate next) => _next = next;

    public async Task InvokeAsync(HttpContext context)
    {
        var headers = context.Response.Headers;

        headers["X-Content-Type-Options"] = "nosniff";
        headers["X-Frame-Options"] = "DENY";
        headers["Referrer-Policy"] = "no-referrer";
        headers["Permissions-Policy"] = "camera=(), microphone=(), geolocation=()";

        // The API only returns JSON, so it never has to load scripts, styles or frames.
        // Swagger UI is excluded because it needs inline scripts and styles.
        if (!context.Request.Path.StartsWithSegments(SwaggerPath))
            headers["Content-Security-Policy"] = "default-src 'none'; frame-ancestors 'none'";

        await _next(context);
    }
}
