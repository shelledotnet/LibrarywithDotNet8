using AccountInquiry.API.Extensions;
using Books.API.AttributeUsed;
using Books.API.Extensions;
using Books.API.Filter;
using Books.API.Filters;
using Books.Domain.DbContexts;
using Books.Domain.Models;
using Books.Domain.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System.Reflection;
using System.Text.Json.Serialization;


#region configurationBuilder


var configurationBuilder = new ConfigurationBuilder()
                                .AddJsonFile("appsettings.json", false, true)
                                .AddJsonFile("appsettings.Development.json", optional: true, reloadOnChange: true)
                                .Build();

Log.Logger = new LoggerConfiguration()
                 .ReadFrom.Configuration(configurationBuilder)
                 .CreateBootstrapLogger();
//.CreateLogger(); 
#endregion

try
{


    Log.Information("Books starting up...");

    #region Add services to the IOC container.
    var builder = WebApplication.CreateBuilder(args);
    #region add this middleweare .UseSerilog() means enforth we are using serilog as our loger replacing the default logger from Dotnet

    #endregion
    builder.Host.UseSerilog((context, configurationBuilder) => configurationBuilder
                                                             .ReadFrom.Configuration(context.Configuration));

    var projectOptions = builder.Configuration.GetSection(nameof(ProjectOptions)).Get<ProjectOptions>();


    if (projectOptions == null)
    {
        Console.WriteLine("ProjectOptions is null");
    }
    else
    {
        Console.WriteLine($"Option1: {projectOptions.ValidAudiences}, Option2: {projectOptions.ValidIssuer}");
    }




    builder.Services.AddControllers(configure =>
    {
        configure.ReturnHttpNotAcceptable = true;
    }).AddJsonOptions(x =>
                x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles)
     .AddXmlDataContractSerializerFormatters()//for xml output
    .ConfigureApiBehaviorOptions(options =>
    {

        options.SuppressModelStateInvalidFilter = true;
    });//for custom message  different from the workload
    builder.Services.AddScoped<IClientHeader, ClientHeader>();

    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(c =>
    {

        c.SwaggerDoc("v1", new()
        {
            Title = "Books API",
            Version = "v1",
            Description = "Through this API you can access all authors and books",
            Contact = new()
            {
                Email = "support@fcmb.com",
                Name = "Digital fcmb",
                Url = new Uri(projectOptions.ContactURL)
            },
            License = new()
            {
                Name = "Fcmb License",
                Url = new Uri(projectOptions.LicenseURL)
            }
        });
        c.AddSwaggerApiKeySecurity();
        c.AddSwaggerApiKeyAuthorization();
        c.OperationFilter<CustomHeaderSwaggerAttribute>();

        var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
        var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
        c.IncludeXmlComments(xmlPath);
    });

    builder.Services.AddOptions<ProjectOptions>()
                  .BindConfiguration(nameof(ProjectOptions))
                  .ValidateDataAnnotations()
                   .Validate(options =>
                   {
                       if (options.XApiKey != projectOptions.XApiKeyMap) return false;
                       return true;
                   })
                 .ValidateOnStart();

    builder.Services.AddCors(options =>
    {
        options.AddDefaultPolicy(builder =>
         builder.AllowAnyOrigin()
                      .AllowAnyMethod()
                      .AllowAnyHeader());
    });
    builder.Services.AddDbContextFactory<BookContext>(
    opt => opt.UseSqlServer(builder.Configuration.GetConnectionString("BooksConnection")));
    //please note ensure to create a class that inherit IDesignTimeDbContextFactory<BookContext>

    builder.Services.AddAutoMapper(typeof(Program).Assembly);
    builder.Services.AddScoped<IBooksRepository, BooksRepository>();
    builder.Services.AddApiVersioning(option =>
    {
        option.AssumeDefaultVersionWhenUnspecified = true;
        option.DefaultApiVersion = new ApiVersion(1, 0);
        option.ReportApiVersions = true;
    });
    builder.Services.AddScoped<RequestAuthActionFilter>();
    #endregion

    #region Middlewear HttpRequest Lands  here this Listent to HttpRequest hirachichally (is the link btw Clients and Server)
    var app = builder.Build();
#if DEBUG
    #region This will create the Db and run all pending migrations if not exist when application start***this should be discourage at production
    await EnsureDatabaseIsMigrated(app.Services);
    async Task EnsureDatabaseIsMigrated(IServiceProvider services)
    {
        using var scope = services.CreateScope();//the scope help us to get the BookContext that we have injected
        using var ctx = scope.ServiceProvider.GetService<BookContext>();
        if (ctx is not null)
        {
            await ctx.Database.MigrateAsync();//this method MigrateAsync(); will help create DB if not exist and likewise run all pending migration
        }
    }
    #endregion
#else
#endif



    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
	{
		app.UseSwagger();
        app.UseSwaggerUI(c =>
        {

            c.SwaggerEndpoint("/swagger/v1/swagger.json", "Books API");
            c.DefaultModelExpandDepth(2);
            c.DefaultModelRendering(Swashbuckle.AspNetCore.SwaggerUI.ModelRendering.Model);
            c.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.None);
            c.EnableDeepLinking();
            c.DisplayOperationId();
        });
        app.UseDeveloperExceptionPage();
    }
    else if (!app.Environment.IsDevelopment())
    {
        app.UseExceptionHandler(appBuilder =>
        {
            appBuilder.Run(async context =>
            {
                context.Response.StatusCode = 500;
                await context.Response.WriteAsync("An Unexpected fault happened. Try again later.");
            });
        });
    }

    app.UseCorrelationId();
    app.UseHttpsRedirection();
    app.UseCors();//add this after UserRouting and before UseEndpoints  or UseAuthorization();
    app.UseMiddleware(typeof(CustomResponseHeaderMiddleware));
    app.UseRequesResponse();
    app.UseSerilogRequestLogging(opts => opts.EnrichDiagnosticContext = LogHelper.EnrichFromRequest);
    app.UseResponseCaching();//this-to-be-added-b4-app.MapControllers()
    app.UseAuthentication();
    app.UseAuthorization();

    app.MapControllers();

    app.Run();
    #endregion
}
catch (Exception ex)
{
    string type = ex.GetType().Name;
    if (type.Equals("StopTheHostException", StringComparison.OrdinalIgnoreCase)) throw;
    Log.Fatal(ex, "Books failed to start corretly , Host terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}
