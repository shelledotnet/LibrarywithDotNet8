
using Books.domain.Models;
using Books.Domain.Enumerations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace YouVerify.API.Filters;

public class AuthFilterAttribute : IAsyncActionFilter
{

    public async Task OnActionExecutionAsync(ActionExecutingContext context,
        ActionExecutionDelegate next)
    {
        //To do : before the action executes  
        ServiceResponse<List<string>> genericResponse = new();
        genericResponse.Code = Convert.ToInt32(ResponseEnum.SecurityViolation);
        genericResponse.Message = "Security Violation";
        List<string> errorMessages = new List<string>();

        try
        {

            bool isError = false;

            string _clientId = "";
            _clientId = context.HttpContext.Request.Headers["client_id"];

            if (string.IsNullOrEmpty(_clientId) || string.IsNullOrWhiteSpace(_clientId))
            {

                isError = true;
                errorMessages.Add("Client Id is required");
                Log.Information("No Client Id Supplied");

            }

            string _correlationId = "";
            _correlationId = context.HttpContext.Request.Headers["x-correlation-id"];

            Guid xCorrelationId;
            if (!string.IsNullOrEmpty(_correlationId) && !string.IsNullOrWhiteSpace(_correlationId) && !Guid.TryParse(_correlationId, out xCorrelationId))
            {

                isError = true;
                errorMessages.Add("x-correlation-id must be a Guid");
                Log.Information("x-correlation-id must be a Guid");
                



            }

            string _productId = "";
            _productId = context.HttpContext.Request.Headers["product_Id"];




            if (string.IsNullOrEmpty(_productId) || string.IsNullOrWhiteSpace(_productId))
            {



                isError = true;
                errorMessages.Add("productId is required");
                Log.Information("No productId Supplied");



            }
            if (isError)
            {
                genericResponse.Code = 63;
                genericResponse.Message = "Security Violation";
                genericResponse.Data = errorMessages;
                context.Result = new JsonResult(new { HttpStatusCode.Unauthorized, genericResponse });

                return;


            }

        }
        catch (Exception ex)
        {
            errorMessages.Add("An Error Occured");
            genericResponse.Code = 95;
            genericResponse.Message = "System Malfunction";
            genericResponse.Data = errorMessages;
            context.Result = new JsonResult(new { HttpStatusCode.Unauthorized, genericResponse });
            Log.Error(ex, "Error Validating headers");
            return;
        }
        await next();
        //To do : after the action executes  
    }
}


