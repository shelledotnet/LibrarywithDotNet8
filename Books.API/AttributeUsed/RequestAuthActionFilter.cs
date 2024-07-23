using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using Books.API.Extensions;

namespace Books.API.AttributeUsed
{
    public class RequestAuthActionFilter : IActionFilter
    {


        private readonly ILogger _logger;

        public RequestAuthActionFilter(ILoggerFactory loggerFactory)
        {

            _logger = loggerFactory.CreateLogger<RequestAuthActionFilter>();

        }
        public void OnActionExecuting(ActionExecutingContext context)
        {
            
            if (!context.ModelState.IsValid)
            {
                context.Result = new BadRequestObjectResult(context.ModelState.GetApiResponse());
            }

        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            //this method will be called after onActionExecuted (i dont want anything for now)
        }

    }
}
