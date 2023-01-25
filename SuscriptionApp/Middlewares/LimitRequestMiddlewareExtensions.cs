using SuscriptionApp;
using SuscriptionApp.DTOs;

namespace SuscriptionApp.Middlewares
{
    public static class LimitRequestMiddlewareExtensions
    {
        public static IApplicationBuilder UseLimitRequests(this IApplicationBuilder app)
        {
            return app.UseMiddleware<LimitRequestMiddleware>();
        }
    }
}

public class LimitRequestMiddleware
{
    private readonly RequestDelegate next;
    private readonly IConfiguration configuration;

    public LimitRequestMiddleware(RequestDelegate next, IConfiguration configuration)
    {
        this.next = next;
        this.configuration = configuration;
    }

    public async Task InvokeAsync(HttpContext httpContext, ApplicationDbContext context)
    {
        var limitRequestConfiguration = new LimitRequestConfiguration();

        configuration.GetRequiredSection("RequestLimits").Bind(limitRequestConfiguration);
        var keystringValue = httpContext.Request.Headers["X-Api-Key"];

        if (keystringValue.Count == 0)
        {
            httpContext.Response.StatusCode = 400;
            await httpContext.Response.WriteAsync("You should provide the key: X-Api-Key");
            return;
        }

        await next(httpContext);
    }
}
