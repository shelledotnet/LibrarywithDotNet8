using Azure;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using Books.domain.Models;
using Books.Domain.Models;
using Microsoft.Extensions.Options;
using Books.Domain.Service;
using System.Text.RegularExpressions;

namespace Books.API.Filter;

public partial class RequestAuthActionFilter : IActionFilter
{
    private readonly ProjectOptions _projectOptions;
    private readonly IClientHeader _clientHeader;
    private readonly ILogger<RequestAuthActionFilter> _logger;
    public RequestAuthActionFilter(IOptionsMonitor<ProjectOptions> projectOptions, ILogger<RequestAuthActionFilter> logger,
        IClientHeader clientHeader)
    {
        _projectOptions = projectOptions.CurrentValue;
        _logger = logger;
        _clientHeader = clientHeader;
    }

    

    public void OnActionExecuting(ActionExecutingContext context)
    {
        //this method will be executed before execution of an action method 
        var headers = context.HttpContext.Request.Headers;
        string clientIdentifier = null;
        string correlationId = null;


        if (headers.TryGetValue("product_id", out Microsoft.Extensions.Primitives.StringValues value) && IsXXSString(value))
        {
            
                var output = new ServiceFailedResponse() { Code = 400, Message = $"invalid value of product_id" };
                context.Result = new BadRequestObjectResult(output);
                return;
        }



        if (!headers.ContainsKey("client_id"))
        {
            var output = new ServiceFailedResponse() { Code = 400, Message = "Header, 'client_id', is requred" };
            context.Result = new BadRequestObjectResult(output);
            return;
        }
        else
        {
            clientIdentifier = headers["client_id"];

            if (string.IsNullOrEmpty(clientIdentifier))
            {
                var output = new ServiceFailedResponse() {Code = 400, Message = "client_id value is missing" };
                context.Result = new BadRequestObjectResult(output);
                return;
            }
            else
            {

                if (IsXXSString(clientIdentifier))
                {
                    var output = new ServiceFailedResponse() { Code = 400, Message = $"invalid value of client_id" };
                    context.Result = new BadRequestObjectResult(output);
                    return;
                }

                
                int.TryParse(_projectOptions.ClientIdIdMaxLength, out int clientIdIdMaxLength);
                if (clientIdentifier.Length > clientIdIdMaxLength)
                {
                    var output = new ServiceFailedResponse() { Code = 400, Message = $"client_id passed exceeds maximum length of {clientIdIdMaxLength}" };
                    context.Result = new BadRequestObjectResult(output);
                    return;
                }
            }

            _logger.LogInformation("Header contained clientIdentifier: {@clientIdentifier}", clientIdentifier);

        }


        if (headers.ContainsKey("x-correlation-id"))
        {
            correlationId = headers["x-correlation-id"];
            if (string.IsNullOrEmpty(correlationId))
            {
                var output = new ServiceFailedResponse() { Code = 400, Message = "Empty correlationId value supplied" };
                context.Result = new BadRequestObjectResult(output);
                return;
            }

            correlationId = correlationId.Trim();
            int.TryParse(_projectOptions.CorrelationIdMaxLength, out int CorrelationIdMaxLength);
            if (correlationId.Length > CorrelationIdMaxLength)
            {
                var output = new ServiceFailedResponse() { Code = 400, Message = $"x-correlation-id passed exceeds maximum length of {CorrelationIdMaxLength}" };
                context.Result = new BadRequestObjectResult(output);
                return;
            }
            if (!IsGUID(correlationId))
            {
                var output = new ServiceFailedResponse() { Code = 400, Message = $"x-correlation-id passed isn't guid {correlationId}" };
                context.Result = new BadRequestObjectResult(output);
                return;
            }
        }
        else if(!headers.ContainsKey("x-correlation-id"))
        {
                var output = new ServiceFailedResponse() { Code = 400, Message = "Header, 'x-correlation-id', is missing" };
                context.Result = new BadRequestObjectResult(output);
                return;
            
        }
        
        _clientHeader.clientId(clientIdentifier.Trim());
        _clientHeader.correlationId(correlationId?.Trim());

        // this is better to use as global but i have already used the above in a lot of places so it won't be easy changing it to below...//
        Environment.SetEnvironmentVariable("clientId", clientIdentifier.Trim());
        Environment.SetEnvironmentVariable("correlationId", correlationId.Trim());
        ///////////////////////////////////////////////////////////////////////////
        var clientIPAddr = context.HttpContext.Connection.RemoteIpAddress?.ToString();
        context.HttpContext.Request.Headers.Append("clientIPAddr", clientIPAddr);

    }
    public void OnActionExecuted(ActionExecutedContext context)
    {
        //this method will be executed after an action method has executed 
    }
    private static bool IsGUID(string str)
    {
        if (Guid.TryParse(str, out Guid xCorrelationId))
            return true;
        return false;
    }
    public bool IsXXSString(string str)
    {
        if (Validate().Match(str.Trim()).Success)
            return true;
        return false;

    }

    [GeneratedRegex(@"[^\w\.-]")]
    private static partial Regex Validate();
}


