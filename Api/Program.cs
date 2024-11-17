using Api;
using Api.Middlewares;
using Application.Services;
using Application.Services.Users;
using Domain.Entities;
using Domain.Validators;
using FluentValidation;
using Infrastructure;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
var configuration = builder.Configuration;

services.AddSwaggerGen();
services.AddControllers();

services.AddDbContext<JournalDbContext>(
    options => options.UseNpgsql(configuration.GetConnectionString(nameof(JournalDbContext)))
);

services.AddTransient<ExceptionMiddleware>();

services.AddTransient<IValidator<User>, UserValidator>();
services.AddTransient<IValidator<Group>, GroupValidator>();
services.AddTransient<IValidator<Table>, TableValidator>();

services.AddScoped<IRepository<User>, UsersRepository>();
services.AddScoped<IRepository<Group>, GroupsRepository>();
services.AddScoped<IRepository<Table>, TablesRepository>();

services.AddScoped<IUsersService, UsersService>();
services.AddScoped<GroupsService>();
services.AddScoped<TablesService>();

services.AddAutoMapper(typeof(MappingProfile));

var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<ExceptionMiddleware>();

app.MapControllers();
app.Run();