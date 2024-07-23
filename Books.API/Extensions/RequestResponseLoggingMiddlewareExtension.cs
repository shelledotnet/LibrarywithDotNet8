namespace Books.API.Extensions
{
    public static class RequestResponseLoggingMiddlewareExtension
    {
       
          public static IApplicationBuilder UseRequesResponse(
            this IApplicationBuilder builder)
        {
          
            return builder.UseMiddleware<RequestResponseLoggingMiddleware>();
        }
         

    }
}
