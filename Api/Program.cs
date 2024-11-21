using Api;
using Api.Extensions;
using Api.Middlewares;
using Application.Services.Groups;
using Application.Services.Tables;
using Application.Services.UserDiffs;
using Application.Services.Users;
using Domain.Entities;
using Domain.Validators;
using FluentValidation;
using GoogleSheetParser.GoogleSheet;
using GoogleSheetParser.Parser;
using Infrastructure;
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
services.AddSingleton<IPasswordHasher, PasswordHasherBCrypt>();

services.AddTransient<ExceptionMiddleware>();

services.AddTransient<IValidator<User>, UserValidator>();
services.AddTransient<IValidator<Group>, GroupValidator>();
services.AddTransient<IValidator<Table>, TableValidator>();
services.AddTransient<IValidator<UserDiff>, UserDiffValidator>();

services.AddScoped<IUsersRepository, UsersRepository>();
services.AddScoped<IRepository<Group>, GroupsRepository>();
services.AddScoped<IRepository<Table>, TablesRepository>();
services.AddScoped<IRepository<UserDiff>, UserDiffRepository>();

services.AddScoped<IUsersService, UsersService>();
services.AddScoped<IGroupsService, GroupsService>();
services.AddScoped<ITablesService, TablesService>();
services.AddScoped<IUserDiffsService, UserDiffService>();

services.AddSingleton<GoogleSheetManager>(serviceProvide =>
{
    return new GoogleSheetManager(configuration.GetConnectionString("CredentialsPath")!);
});

services.AddSingleton<ExcelParser>();

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