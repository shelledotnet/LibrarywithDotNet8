using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Books.API.Filters
{
#pragma warning disable 

    public class CustomHeaderSwaggerAttribute :IOperationFilter 
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            if (operation.Parameters == null)
                operation.Parameters = new List<OpenApiParameter>();

            operation.Parameters.Add(
                new OpenApiParameter
                {
                    Name = "x-correlation-id",
                    In = ParameterLocation.Header,
                    Required = false,
                    Schema = new OpenApiSchema
                    {
                        Type = "String"
                    }
                }
            );

            operation.Parameters.Add(
                new OpenApiParameter
                {
                    Name = "client_id",
                    In = ParameterLocation.Header,
                    Required = false,
                    Schema = new OpenApiSchema
                    {
                        Type = "String"
                    }
                }
            );

            operation.Parameters.Add(
               new OpenApiParameter
               {
                   Name = "product_id",
                   In = ParameterLocation.Header,
                   Required = false,
                   Schema = new OpenApiSchema
                   {
                       Type = "String"
                   }
               }
           );

        }
    }
}
