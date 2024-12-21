using System.Text;
using Api.Handlers;
using Application;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;

namespace Api.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApiAuthentication(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<AuthOptions>(configuration.GetSection(nameof(AuthOptions)));

        var authOptions = configuration.GetSection(nameof(AuthOptions)).Get<AuthOptions>();

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme,
                options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(authOptions!.JwtSecretKey))
                    };

                    options.Events = new JwtBearerEvents
                    {
                        OnMessageReceived = context =>
                        {
                            context.Token = context.Request.Cookies[authOptions.CookieName];
                            return Task.CompletedTask;
                        }
                    };
                }
            );
        return services;
    }

    public static IServiceCollection AddDenyAuthenticatedPolicy(this IServiceCollection services)
    {
        services.AddAuthorizationBuilder()
            .AddPolicy("DenyAuthenticated", policy =>
                policy.Requirements.Add(new DenyAuthenticatedRequirement())
            );

        services.AddScoped<IAuthorizationHandler, DenyAuthenticatedHandler>();

        return services;
    }

    public static IServiceCollection AddSuperAdminPolicy(this IServiceCollection services)
    {
        services.AddAuthorizationBuilder()
            .AddPolicy("SuperAdmin", policy =>
                policy.Requirements.Add(new SuperAdminRequirement())
            );

        services.AddScoped<IAuthorizationHandler, SuperAdminHandler>();

        return services;
    }

    public static IServiceCollection AddPersonalDataAccessPolicy(this IServiceCollection services)
    {
        services.AddAuthorizationBuilder()
            .AddPolicy("PersonalDataAccess", policy =>
                policy.Requirements.Add(new PersonalDataAccessRequirement())
            );

        services.AddScoped<IAuthorizationHandler, PersonalDataAccessHandler>();

        return services;
    }

    public static IServiceCollection AddUserIsGroupMemberPolicy(this IServiceCollection services)
    {
        services.AddAuthorizationBuilder()
            .AddPolicy("UserIsGroupMember", policy =>
                policy.Requirements.Add(new UserIsGroupMemberRequirement())
            );
        
        services.AddScoped<IAuthorizationHandler, UserIsGroupMemberHandler>();

        return services;
    }

    public static IServiceCollection AddCombinedPolicies(this IServiceCollection services)
    {
        services.AddScoped<IAuthorizationHandler, CombinedAuthorizationHandler>();

        services.AddAuthorizationBuilder()
            .AddPolicy("SuperAdminOrPersonalDataAccess", policy =>
                policy.RequireCombinedPolicies("SuperAdmin", "PersonalDataAccess"));
        
        services.AddAuthorizationBuilder()
            .AddPolicy("SuperAdminOrUserIsGroupMember", policy =>
                policy.RequireCombinedPolicies("SuperAdmin", "UserIsGroupMember"));

        return services;
    }
}