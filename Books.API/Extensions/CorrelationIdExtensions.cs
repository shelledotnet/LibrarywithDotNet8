
using Books.API.Middlewear;

namespace Books.API.Extensions
{
    public static class CorrelationIdExtensions
    {
        public static IApplicationBuilder UseCorrelationId(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<LogHeaderMiddleware>();
        }
    }
}
