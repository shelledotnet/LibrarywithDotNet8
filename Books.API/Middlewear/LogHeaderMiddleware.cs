
using Microsoft.AspNetCore.Http;
using Serilog.Context;
using System.Net;

namespace Books.API.Middlewear
{
    public class LogHeaderMiddleware
    {

        private readonly RequestDelegate _next;
        private readonly ILogger _logger;



        public LogHeaderMiddleware(RequestDelegate next, ILoggerFactory loggerFactory)
        {
            _next = next;
            _logger = loggerFactory.CreateLogger<LogHeaderMiddleware>();

        }



        #region MyRegion
        public async Task Invoke(HttpContext context)
        {
            try
            {
                string correlationId = null;
                var key = context.Request.Headers.Keys.FirstOrDefault(n => n.ToLower().Equals("x-correlation-id"));
                if (!string.IsNullOrWhiteSpace(key) && !string.IsNullOrEmpty(key))
                {
                    correlationId = context.Request.Headers[key];
                    // _logger.LogInformation("Header contained CorrelationId: {@CorrelationId}", correlationId);
                }
                else
                {
                    correlationId = Guid.NewGuid().ToString();
                    context.Request.Headers.Append("x-correlation-id", correlationId);
                    // _logger.LogInformation("Header contained CorrelationId: {@CorrelationId}", correlationId);

                }
                context.Response.Headers.Append("x-correlation-id", correlationId);

                string clientId = GetHeaderValue("client_id", context);
                string productId = GetHeaderValue("product_id", context);

                context.Response.Headers.Append("client_id", clientId);
                context.Response.Headers.Append("product_id", productId);

                using (LogContext.PushProperty("CorrelationId", correlationId))
                {
                    await _next.Invoke(context);
                }
            }
            catch (Exception ex)
            {

                string message = $"{ex}";
                _logger.LogError(message);
            }
        }

        public string GetHeaderValue(string key, HttpContext context)
        {

            var value = context.Request.Headers.Keys.FirstOrDefault(n => n.ToLower().Equals(key));
            if (!string.IsNullOrWhiteSpace(value))
            {

                return context.Request.Headers[value];


            }
            else
            {
                context.Request.Headers.Append(key, "");

            }

            return "";

        }
        #endregion


    }


}
