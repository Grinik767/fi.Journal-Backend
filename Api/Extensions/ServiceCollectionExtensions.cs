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
                        ValidateIssuer = false, //издатель токена
                        ValidateAudience = false, // аудитория токена
                        ValidateLifetime = true, // время жизни токена
                        ValidateIssuerSigningKey = true, //подделка токена
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(authOptions!.JwtSecretKey)) // как проверять подлинность
                    };

                    options.Events = new JwtBearerEvents //ищем куку по её имени, если всё норм - присваиваем 
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
    
    /// <summary>
    /// Политика, запрещающая доступ для зарегистрированных пользователей (для страницы входа)
    /// </summary>
    public static IServiceCollection AddDenyAuthenticatedPolicy(this IServiceCollection services)
    {
        services.AddAuthorizationBuilder()
            .AddPolicy("DenyAuthenticated", policy =>
                policy.Requirements.Add(new DenyAuthenticatedRequirement())
            );

        services.AddScoped<IAuthorizationHandler, DenyAuthenticatedHandler>();

        return services;
    }
    
    /// <summary>
    /// Политики для админов
    /// </summary>
    public static IServiceCollection AddSuperAdminPolicy(this IServiceCollection services)
    {
        services.AddAuthorizationBuilder()
            .AddPolicy("SuperAdmin", policy =>
                policy.Requirements.Add(new SuperAdminRequirement())
            );

        services.AddScoped<IAuthorizationHandler, SuperAdminHandler>();

        return services;
    }
    
    /// <summary>
    /// Политика для персональных данных 
    /// </summary>
    public static IServiceCollection AddPersonalDataAccessPolicy(this IServiceCollection services)
    {
        services.AddAuthorizationBuilder()
            .AddPolicy("PersonalDataAccess", policy =>
                policy.Requirements.Add(new PersonalDataAccessRequirement())
            );

        services.AddScoped<IAuthorizationHandler, PersonalDataAccessHandler>();

        return services;
    }
    
    /// <summary>
    /// Политика для юзеров, находящихся в группе
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection AddUserIsGroupMemberPolicy(this IServiceCollection services)
    {
        services.AddAuthorizationBuilder()
            .AddPolicy("UserIsGroupMember", policy =>
                policy.Requirements.Add(new UserIsGroupMemberRequirement())
            );
        
        services.AddScoped<IAuthorizationHandler, UserIsGroupMemberHandler>();

        return services;
    }
    
    /// <summary>
    /// Комбинированные политики
    /// </summary>
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