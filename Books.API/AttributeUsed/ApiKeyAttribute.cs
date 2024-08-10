using Books.domain.Models;
using Books.Domain.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;
using Serilog;
using System.Text;

namespace Books.API.AttributeUsed
{

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public class ApiKeyAttribute : Attribute, IAuthorizationFilter
    {

        private const string _ApiKeyName = "XApiKey";
        private readonly ProjectOptions _optionsMonitor;

        public ApiKeyAttribute(IOptionsMonitor<ProjectOptions> optionsMonitor)
        {
            _optionsMonitor = optionsMonitor.CurrentValue;
        }


        public void OnAuthorization(AuthorizationFilterContext context)
        {
            try
            {
                var httpContext = context.HttpContext;

                

                #region Old-Implementation
                //var apiKeyPresentInHeader = httpContext.Request.Headers.TryGetValue(_ApiKeyName, out var expectedApiKeyValue);

                //byte[] expectedValue = Convert.FromBase64String(expectedApiKeyValue);

                //if (apiKeyPresentInHeader && _optionsMonitor.XApiKey == Encoding.UTF8.GetString(expectedValue)
                //    || httpContext.Request.Path.StartsWithSegments("/swagger"))
                //{
                //    return;
                //}
                //ServiceFailedResponse response = new() { Message = "unauthorised", IsSuccess = false };
                //context.Result = new UnauthorizedObjectResult(response); 
                #endregion

                
                if (!httpContext.Request.Headers.TryGetValue(_ApiKeyName, out var expectedApiKeyValue))
                {
                    ServiceFailedResponse response = new() { Message = "Api Key not found..", IsSuccess = false };
                    context.Result = new UnauthorizedObjectResult(response);
                }

                //expectedApiKeyValue to be in Base64String
                byte[] expectedValue = Convert.FromBase64String(expectedApiKeyValue);
                
                if (!string.Equals(_optionsMonitor.XApiKey, Encoding.UTF8.GetString(expectedValue), StringComparison.Ordinal))
                {
                    ServiceFailedResponse response = new() { Message = "Invalid API Key.", IsSuccess = false };
                    context.Result = new UnauthorizedObjectResult(response);
                }
            
            }
            catch (FormatException fx)
            {
                Log.Fatal(fx, "Books-RPG failed to start corretly , ApiKeyName");
                ServiceFailedResponse response = new() { Message = "The header input value is not a valid Base-64 string", IsSuccess = false };
                context.Result = new UnauthorizedObjectResult(response);
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Books-RPG failed to start corretly , ApiKeyName");
                ServiceFailedResponse response = new() { Message = "unauthorised", IsSuccess = false };
                context.Result = new UnauthorizedObjectResult(response);

            }
        }
    }
}
