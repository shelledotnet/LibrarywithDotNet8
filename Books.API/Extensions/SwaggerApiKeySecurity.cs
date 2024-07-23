using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Books.API.Extensions
{
    public static class SwaggerApiKeySecurity
    {
        public static void AddSwaggerApiKeySecurity(this SwaggerGenOptions c)
        {
            #region BasicAuth
            c.AddSecurityDefinition("ApiKey", new OpenApiSecurityScheme
            {
                Description = "Apikey must appear in header",
                Type = SecuritySchemeType.ApiKey,
                Name = "XApiKey",
                In = ParameterLocation.Header,
                Scheme = "ApiKeyScheme"
            });
           

            var key = new OpenApiSecurityScheme()
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "ApiKey"
                },
                In = ParameterLocation.Header,
              
            };
            var requirement = new OpenApiSecurityRequirement
                {
                   { key, new List<string>() }
                };

            c.AddSecurityRequirement(requirement);
            #endregion

        }
    }

    public static class SwaggerApiKeyAuthorization
    {
        public static void AddSwaggerApiKeyAuthorization(this SwaggerGenOptions c)
        {
            #region Authorization
            c.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
            {
                Description = "Standard Authorization header using the Bearer scheme (\"bearer {token}\")",
                Type = SecuritySchemeType.ApiKey,
                Name = "Authorization",
                In = ParameterLocation.Header,
                BearerFormat = "JWT",
                Scheme = "Bearer"
            });
            c.OperationFilter<SecurityRequirementsOperationFilter>();
            #endregion

        }
    }
}
