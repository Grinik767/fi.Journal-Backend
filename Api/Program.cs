using Api;
using Api.Extensions;
using Api.Middlewares;
using Application.Services.Groups;
using Application.Services.Tables;
using Application.Services.Users;
using Domain.Entities;
using Domain.Validators;
using FluentValidation;
using Infrastructure;
using Infrastructure.Jwt;
using Infrastructure.PasswordHasher;
using Infrastructure.Repositories;
using Infrastructure.Repositories.Users;
using Microsoft.AspNetCore.CookiePolicy;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
var configuration = builder.Configuration;

services.AddSwaggerGen();
services.AddControllers();

services.AddDbContext<JournalDbContext>(
    options => options.UseNpgsql(configuration.GetConnectionString(nameof(JournalDbContext)))
);

services.AddApiAuthentication(configuration);

services.AddTransient<ExceptionMiddleware>();

services.AddSingleton<IPasswordHasher, PasswordHasherBCrypt>();
services.AddSingleton<JwtProvider>();

services.AddTransient<IValidator<User>, UserValidator>();
services.AddTransient<IValidator<Group>, GroupValidator>();
services.AddTransient<IValidator<Table>, TableValidator>();

services.AddScoped<IUsersRepository, UsersRepository>();
services.AddScoped<IRepository<Group>, GroupsRepository>();
services.AddScoped<IRepository<Table>, TablesRepository>();

services.AddScoped<IUsersService, UsersService>();
services.AddScoped<IGroupsService, GroupsService>();
services.AddScoped<ITablesService, TablesService>();

services.AddAutoMapper(typeof(MappingProfile));

var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<ExceptionMiddleware>();

app.UseCookiePolicy(new CookiePolicyOptions
{
    MinimumSameSitePolicy = SameSiteMode.Strict,
    HttpOnly = HttpOnlyPolicy.Always,
    Secure = CookieSecurePolicy.Always
});

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.Run();