using Books.API.AttributeUsed;
using System.Runtime.CompilerServices;

namespace Books.API.Extensions
{
    public static class ApiKeyMiddlewareExtension
    {
        public static IApplicationBuilder UseApiKey(
            this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ApiKeyAttribute>();
        }
    }
}
