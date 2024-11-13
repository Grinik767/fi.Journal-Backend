using Api;
using Api.Middlewares;
using Application.Services;
using Infrastructure;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using GoogleSheetParser.GoogleSheet;
using GoogleSheetParser.Parser;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
var configuration = builder.Configuration;

services.AddSwaggerGen();
services.AddControllers();

services.AddDbContext<JournalDbContext>(
    options => options.UseNpgsql(configuration.GetConnectionString(nameof(JournalDbContext)))
);

services.AddTransient<ExceptionMiddleware>();

services.AddScoped<UsersRepository>();
services.AddScoped<GroupsRepository>();
services.AddScoped<TablesRepository>();

services.AddScoped<UsersService>();
services.AddScoped<GroupsService>();
services.AddScoped<TablesService>();

Console.WriteLine();

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

app.MapControllers();
app.Run();