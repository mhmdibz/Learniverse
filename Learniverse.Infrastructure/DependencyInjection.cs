using Learniverse.Application.Interfaces.Common;
using Learniverse.Application.Interfaces.Identity;
using Learniverse.Infrastructure.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Learniverse.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDataProtection();
        services.AddHttpContextAccessor();
        services.AddScoped<ICurrentUserService, CurrentUserService>();
        services.AddScoped<IIdentityService, IdentityService>();
        services.AddScoped<ITokenService, TokenService>();

        services.AddOptions<JwtOptions>()
            .BindConfiguration(JwtOptions.SectionName)
            .ValidateOnStart();
        services.AddOptions<BootstrapAdminOptions>()
    .BindConfiguration(BootstrapAdminOptions.SectionName)
    .ValidateOnStart();
        services.AddAuthentication(
        JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.MapInboundClaims = false;
    });
        services.AddAuthorization();

        services.AddOptions<JwtBearerOptions>(
                JwtBearerDefaults.AuthenticationScheme)
            .Configure<IOptions<JwtOptions>>(
                (bearerOptions, jwtOptions) =>
                {
                    bearerOptions.TokenValidationParameters =
                        new TokenValidationParameters
                        {
                            ValidateIssuer = true,
                            ValidateAudience = true,
                            ValidateLifetime = true,
                            ValidateIssuerSigningKey = true,

                            ValidIssuer = jwtOptions.Value.Issuer,
                            ValidAudience = jwtOptions.Value.Audience,

                            IssuerSigningKey =
                                new SymmetricSecurityKey(
                                    Encoding.UTF8.GetBytes(
                                        jwtOptions.Value.SecretKey))
                        };
                });

        services.AddSingleton<
            IValidateOptions<JwtOptions>,
            JwtOptionsValidator>();
        services.AddSingleton<
        IValidateOptions<BootstrapAdminOptions>,
        BootstrapAdminOptionsValidator>();
        return services;
    }
}