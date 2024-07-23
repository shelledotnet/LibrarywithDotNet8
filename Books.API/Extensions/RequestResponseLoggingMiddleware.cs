using Books.Domain.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.Options;
using System.Text;

namespace Books.API.Extensions
{
    public class RequestResponseLoggingMiddleware
    {  
        private const string _ApiKeyName = "XApiKey";
        private readonly ProjectOptions _optionsMonitor;
        private readonly RequestDelegate _next;



        public RequestResponseLoggingMiddleware(RequestDelegate next,IOptionsMonitor<ProjectOptions>  optionsMonitor)
        {
            _next = next;
            _optionsMonitor = optionsMonitor.CurrentValue;
        }

        
        public async Task AuthAsync(HttpContext context)
        {
            var apiKeyPresentInHeader = context.Request.Headers.TryGetValue(_ApiKeyName, out var expectedApiKey);
            if ((apiKeyPresentInHeader && _optionsMonitor.XApiKey == expectedApiKey)
                || context.Request.Path.StartsWithSegments("/swagger"))
            {
                await _next(context);
                return;
            }

            context.Response.StatusCode = 401;
            await context.Response.WriteAsync("Invalid Api Key");
         
        }
         
         




        public async Task Invoke(HttpContext context)
        {
            // Read and log request body data
            
            string requestBodyPayload = await ReadRequestBody(context.Request);
            LogHelper.RequestPayload = requestBodyPayload;

            // Read and log response body data
            // Copy a pointer to the original response body stream
            var originalResponseBodyStream = context.Response.Body;

            // Create a new memory stream...
            using (var responseBody = new MemoryStream())
            {
                // ...and use that for the temporary response body
                context.Response.Body = responseBody;

                await _next(context);

               
              //  await AuthAsync(context);

                await responseBody.CopyToAsync(originalResponseBodyStream);
            }
        }

        private async Task<string> ReadRequestBody(HttpRequest request)
        {
            HttpRequestRewindExtensions.EnableBuffering(request);

            var body = request.Body;
            var buffer = new byte[Convert.ToInt32(request.ContentLength)];
            await request.Body.ReadAsync(buffer, 0, buffer.Length);
            string requestBody = Encoding.UTF8.GetString(buffer);
            body.Seek(0, SeekOrigin.Begin);
            request.Body = body;

            return $"{requestBody}";
        }
    }
}
