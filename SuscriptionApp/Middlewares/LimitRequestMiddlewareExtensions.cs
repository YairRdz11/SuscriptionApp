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

        var route = httpContext.Request.Path.ToString().ToLower();

        var isRouteInWithList = limitRequestConfiguration.WhiteListRoutes.Any(x => route.Contains(x.ToLower()));
        if (isRouteInWithList)
        {
            await next(httpContext);
            return;
        }

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

        var keyDB = await context.KeysAPI
            .Include(x => x.DomainRestrictions)
            .Include(x => x.IPRestrictions)
            .Include (x => x.User)
            .FirstOrDefaultAsync(x => x.Key == key);

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
        else if(keyDB.User.BadUser)
        {
            httpContext.Response.StatusCode = 400;
            await httpContext.Response.WriteAsync("User has not paid");
            return;
        }
        var passRestrictions = RequestPassAnyRestriction(keyDB, httpContext);

        if (!passRestrictions)
        {
            httpContext.Response.StatusCode = 403;
            return;
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

    private bool RequestPassAnyRestriction(KeyAPI keyAPI, HttpContext httpContext)
    {
        var areThereRestrictions = keyAPI.DomainRestrictions.Any() || keyAPI.IPRestrictions.Any();

        if (!areThereRestrictions)
        {
            return true;
        }

        var requestPassDomainRestrictions = RequestPassDomainRestrictions(keyAPI.DomainRestrictions, httpContext);
        var requestPassIPRestrictions = RequestPassIPRestrictions(keyAPI.IPRestrictions, httpContext);

        return requestPassDomainRestrictions || requestPassIPRestrictions;
    }

    private bool RequestPassIPRestrictions(List<IPRestriction> restrictions,
        HttpContext httpContext)
    {
        if (restrictions == null || restrictions.Count == 0)
        {
            return false;
        }

        var ip = httpContext.Connection.RemoteIpAddress.ToString();

        if(string.IsNullOrEmpty(ip))
        {
            return false;
        }

        return restrictions.Any(x => x.IP == ip);
    }

    private bool RequestPassDomainRestrictions(List<DomainRestriction> domainRestrictions, 
        HttpContext httpContext) 
    {
        if(domainRestrictions == null || domainRestrictions.Count == 0)
        {
            return false;
        }

        var referer = httpContext.Request.Headers["Referer"].ToString();

        if(string.IsNullOrEmpty(referer) )
        {
            return false;
        }

        Uri myUri = new Uri(referer);
        string host = myUri.Host;

        var passRestriction = domainRestrictions.Any(x => x.Domain == host);

        return passRestriction;
    }
}
