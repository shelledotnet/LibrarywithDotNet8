using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using Books.domain.Models;

namespace Books.API;

public class NonProductionActionFilterAttribute : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        IHostEnvironment environment = context.HttpContext.RequestServices.GetRequiredService<IHostEnvironment>();
        if (environment.IsProduction())
        {
            ServiceFailedResponse serviceFailedResponse = new()
            {
                Message = "service not found"
            };
            context.Result = new NotFoundObjectResult(serviceFailedResponse);
            return;
        }

        base.OnActionExecuting(context);
    }
}
