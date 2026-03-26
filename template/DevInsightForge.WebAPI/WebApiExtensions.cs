using DevInsightForge.WebAPI.Extensions;
using DevInsightForge.WebAPI.Filters;
using DevInsightForge.WebAPI.Routing;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Serilog;

namespace DevInsightForge.WebAPI;

public static class WebApiExtensions
{
    public static void AddWebApiServices(this IServiceCollection services)
    {
        services.AddControllers(options =>
        {
            options.Filters.Add<ValidationProblemDetailsFilter>();
            options.Conventions.Add(new RouteTokenTransformerConvention(new KebabCaseParameterTransformer()));
        });
        services.AddHttpContextAccessor();

        services.Configure<ApiBehaviorOptions>(options => options.SuppressModelStateInvalidFilter = true);

        services.AddOpenApiDocumentation();
        services.AddCorsPolicy();

        services.AddExceptionHandler<GlobalExceptionHandler>();
        services.AddProblemDetails();
    }

    public static void UseWebApiServices(this WebApplication app)
    {
        app.UseExceptionHandler();
        app.UseSerilogRequestLogging();

        if (app.Environment.IsDevelopment())
            app.UseOpenApiDocumentation();

        app.UseHttpsRedirection();
        app.UseCors();

        app.MapControllers();
    }
}

