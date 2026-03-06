using DevInsightForge.WebAPI.Common.Models;
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
                ValidateAudience = configuration.GetValue<bool>("JwtSettings:ValidateAudience"),
                ValidAudience = configuration["JwtSettings:ValidAudience"],
                ValidateIssuer = configuration.GetValue<bool>("JwtSettings:ValidateIssuer"),
                ValidIssuer = configuration["JwtSettings:ValidIssuer"],
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JwtSettings:SecretKey"] ?? "Default_Super_Secret_256_Bits_Signing_Key")),
                ValidateLifetime = true,
            };

            options.Events = ConfigureJwtBearerEvents();
        });

        // Enforce authentication globally
        _ = services.AddAuthorization(options => options.FallbackPolicy = new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .Build());

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

                await context.Response.WriteAsJsonAsync(
                    ApiResponse.FailureResponse(
                        (int)HttpStatusCode.Unauthorized,
                        ["Authentication failed. Please provide a valid access token."],
                        "auth.unauthorized"));
            },
            OnForbidden = async context =>
            {
                context.Response.StatusCode = (int)HttpStatusCode.Forbidden;

                await context.Response.WriteAsJsonAsync(
                    ApiResponse.FailureResponse(
                        (int)HttpStatusCode.Forbidden,
                        ["You do not have permission to access this resource."],
                        "authorization.forbidden"));
            }
        };
    }

    #endregion
}
