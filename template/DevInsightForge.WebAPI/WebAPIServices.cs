using DevInsightForge.Application.Abstructions.Core;
using DevInsightForge.WebAPI.Common.Filters;
using DevInsightForge.WebAPI.Extensions;
using DevInsightForge.WebAPI.Services;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace DevInsightForge.WebAPI;

public static class WebAPIServices
{
    public static void AddWebAPIServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Inject Controller Handlers
        services.AddControllers(options =>
        {
            options.Filters.Add<ModelStateValidationFilter>();
        });
        services.AddHttpContextAccessor();

        // Disable inbuild model validators in favor of Fluent Validation
        services.Configure<ApiBehaviorOptions>(options => options.SuppressModelStateInvalidFilter = true);

        // Inject Extension services
        services.AddAuthenticationService(configuration);
        services.AddSwaggerService();
        services.AddCorsService();

        services.AddExceptionHandler<ExceptionHandlerServiceExtension>();

        // Register WebAPI-specific context services
        services.AddScoped<IAuthenticatedUser, AuthenticatedUser>();
    }

    public static void UseWebAPIServices(this WebApplication app)
    {
        // Configure App Pipelines
        app.UseExceptionHandler();
        app.UseSerilogRequestLogging();

        if (app.Environment.IsDevelopment())
            app.UseSwaggerService();

        app.UseHttpsRedirection();
        app.UseCors();

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();
    }
}



