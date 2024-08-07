using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace AccountInquiry.API.Extensions;

public class CustomResponseHeaderMiddleware
{
    private readonly RequestDelegate _next;
    public CustomResponseHeaderMiddleware(RequestDelegate next)
    {
        _next = next;
    }
    public async Task Invoke(HttpContext context)
    {
        //To add Headers AFTER everything you need to do this
        context.Response.OnStarting(state =>
        {
            var httpContext = (HttpContext)state;
            httpContext.Response.Headers.Append("X-Content-Type-Options", "nosniff");
            httpContext.Response.Headers.Append("X-Xss-Protection", "1; mode=block");
            httpContext.Response.Headers.Append("X-Frame-Options", "DENY");
            httpContext.Response.Headers.Append("Cache-Control", "no-cache");
            httpContext.Response.Headers.Append("Strict-Transport-Security", "max-age=31536000");
            httpContext.Response.Headers.Remove("Server");
            //... and so on
            return Task.CompletedTask;
        }, context);

        await _next(context);
    }
}
