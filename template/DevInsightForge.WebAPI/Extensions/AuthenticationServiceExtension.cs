using DevInsightForge.WebAPI.Common.Models;
using DevInsightForge.Infrastructure.Configurations;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using System.Net;
using System.Text;

namespace DevInsightForge.WebAPI.Extensions;

public static class AuthenticationServiceExtension
{
    public static IServiceCollection AddAuthenticationService(this IServiceCollection services, IConfiguration configuration)
    {
        var jwtConfigurations = configuration.GetSection("JwtConfigurations").Get<JwtConfigurations>() ??
            throw new InvalidOperationException("JwtConfigurations not defined");

        // Configure JWT authentication
        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.SaveToken = true;
            options.RequireHttpsMetadata = false;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = jwtConfigurations.ValidateAudience,
                ValidAudience = jwtConfigurations.ValidAudience,
                ValidateIssuer = jwtConfigurations.ValidateIssuer,
                ValidIssuer = jwtConfigurations.ValidIssuer,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtConfigurations.SecretKey)),
                ValidateLifetime = true,
                ClockSkew = TimeSpan.FromMinutes(1)
            };

            options.Events = ConfigureJwtBearerEvents();
        });

        return services;
    }

    #region Authorization Events Definition
    private static JwtBearerEvents ConfigureJwtBearerEvents()
    {
        return new JwtBearerEvents
        {
            OnChallenge = async context =>
            {
                context.HandleResponse();
                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;

                await context.Response.WriteAsJsonAsync(new ApiResponse
                {
                    StatusCode = (int)HttpStatusCode.Unauthorized,
                    Message = ["Authentication failed. Please provide a valid access token."]
                });
            },
            OnForbidden = async context =>
            {
                context.Response.StatusCode = (int)HttpStatusCode.Forbidden;

                await context.Response.WriteAsJsonAsync(new ApiResponse
                {
                    StatusCode = (int)HttpStatusCode.Forbidden,
                    Message = ["You do not have permission to access this resource."]
                });
            }
        };
    }

    #endregion
}
