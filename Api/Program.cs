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
using Infrastructure.Repositories.UserDiffs;
using Infrastructure.Repositories.Users;
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
services.AddScoped<IUsersDiffsRepository, UserDiffsRepository>();

services.AddScoped<IUsersService, UsersService>();
services.AddScoped<IGroupsService, GroupsService>();
services.AddScoped<ITablesService, TablesService>();
services.AddScoped<IUserDiffsService, UserDiffsService>();

services.AddSingleton<GoogleSheetManager>(_ =>
    new GoogleSheetManager(configuration.GetConnectionString("CredentialsPath")!));

services.AddSingleton<ExcelParser>();
services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
        corsPolicyBuilder => corsPolicyBuilder
            .WithOrigins(
                "http://localhost",
                "https://localhost",
                "http://localhost:3003",
                "https://localhost:3003",
                "http://fi-journal.ru",
                "https://fi-journal.ru"
            )
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials());
});

services.AddAutoMapper(typeof(MappingProfile));

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<JournalDbContext>();
    dbContext.Database.Migrate();
}

app.UseSwagger();
app.UseSwaggerUI();

app.UseMiddleware<ExceptionMiddleware>();

app.UseCors("AllowFrontend");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.Run();