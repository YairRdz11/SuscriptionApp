using Microsoft.EntityFrameworkCore;
using SuscriptionApp;
using SuscriptionApp.DTOs;
using SuscriptionApp.Entities;

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
        if(keystringValue.Count > 1)
        {
            httpContext.Response.StatusCode = 400;
            await httpContext.Response.WriteAsync("You should have only one key");
            return;
        }

        var key = keystringValue[0];

        var keyDB = await context.KeysAPI.FirstOrDefaultAsync(x => x.Key == key);

        if (keyDB == null)
        {
            httpContext.Response.StatusCode = 400;
            await httpContext.Response.WriteAsync("The key doesn't exist");
            return;
        }

        if (!keyDB.Enable)
        {
            httpContext.Response.StatusCode = 400;
            await httpContext.Response.WriteAsync("The key is disabled");
            return;
        }
        if(keyDB.KeyType == SuscriptionApp.Enums.KeyType.Free)
        {
            var today = DateTime.Today;
            var tomorrow = today.AddDays(1);

            var countRequestToday = await context.Requests
                .CountAsync(x => 
                x.KeyId == keyDB.Id && x.RequestDate >= today && x.RequestDate < tomorrow);
            if(countRequestToday >= limitRequestConfiguration.RequestsByDayForFree ) 
            {
                httpContext.Response.StatusCode = 429;
                await httpContext.Response.WriteAsync("You have sent the request limit per day, if you want perform more update your subscription for a professional account");
                return;
            }
        }

        var request = new Request()
        {
            KeyId = keyDB.Id,
            RequestDate = DateTime.UtcNow
        };

        context.Add(request);
        await context.SaveChangesAsync();

        await next(httpContext);
    }
}
