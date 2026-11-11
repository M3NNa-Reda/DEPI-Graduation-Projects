using System.Threading.RateLimiting;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using EasyShop.MVC.Services;

namespace EasyShop.MVC;

public static class DependencyInjection
{
    public const string AuthRateLimitPolicy = "auth";

    public static IServiceCollection AddClientServices(this IServiceCollection services)
    {
        services.AddControllersWithViews();
        services.AddScoped<IProductImageStorage, ProductImageStorage>();
        services.AddSingleton(TimeProvider.System);
        services.AddSingleton<LoginAttemptTracker>();

        services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(options =>
            {
                options.Cookie.Name = "EasyShop.Auth";
                options.Cookie.HttpOnly = true;
                options.Cookie.SameSite = SameSiteMode.Lax;
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                options.LoginPath = "/Auth/Login";
                options.AccessDeniedPath = "/Auth/AccessDenied";
                options.ExpireTimeSpan = TimeSpan.FromHours(8);
                options.SlidingExpiration = true;

                // API clients expect a status code, not a redirect to the login page.
                options.Events.OnRedirectToLogin = context =>
                {
                    if (context.Request.Path.StartsWithSegments("/api"))
                    {
                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    }
                    else
                    {
                        context.Response.Redirect(context.RedirectUri);
                    }

                    return Task.CompletedTask;
                };
                options.Events.OnRedirectToAccessDenied = context =>
                {
                    if (context.Request.Path.StartsWithSegments("/api"))
                    {
                        context.Response.StatusCode = StatusCodes.Status403Forbidden;
                    }
                    else
                    {
                        context.Response.Redirect(context.RedirectUri);
                    }

                    return Task.CompletedTask;
                };
            });

        // Slows down password guessing and mass sign-ups: 10 attempts per minute per IP address.
        services.AddRateLimiter(options =>
        {
            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
            options.AddPolicy(AuthRateLimitPolicy, httpContext =>
                RateLimitPartition.GetFixedWindowLimiter(
                    httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
                    _ => new FixedWindowRateLimiterOptions
                    {
                        PermitLimit = 10,
                        Window = TimeSpan.FromMinutes(1),
                        QueueLimit = 0
                    }));
        });

        return services;
    }

    // Browser-side protections that cost nothing: no MIME sniffing, no framing (clickjacking),
    // no leaking full URLs in the Referer header, no unneeded device features, and a policy
    // that blocks plugins and form posts to other sites. Inline scripts are still allowed
    // because a few pages use them.
    public static WebApplication UseSecurityHeaders(this WebApplication app)
    {
        app.Use(async (context, next) =>
        {
            var headers = context.Response.Headers;
            headers["X-Content-Type-Options"] = "nosniff";
            headers["X-Frame-Options"] = "DENY";
            headers["Referrer-Policy"] = "strict-origin-when-cross-origin";
            headers["Permissions-Policy"] = "camera=(), microphone=(), geolocation=(), payment=()";
            headers["Content-Security-Policy"] =
                "default-src 'self'; img-src 'self' data: https:; style-src 'self' 'unsafe-inline'; " +
                "script-src 'self' 'unsafe-inline'; object-src 'none'; base-uri 'self'; " +
                "form-action 'self'; frame-ancestors 'none'";

            await next();
        });

        return app;
    }

    public static WebApplication AddClientPipeline(this WebApplication app)
    {
        app.UseRateLimiter();
        // Serves pictures that admins upload while the site is running; MapStaticAssets only knows build-time files.
        app.UseStaticFiles();
        app.MapStaticAssets();
        app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}")
            .WithStaticAssets();
        return app;
    }
}
