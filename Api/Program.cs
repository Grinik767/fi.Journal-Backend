using Api;
using Application.Services;
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

services.AddScoped<UsersRepository>();

services.AddScoped<UserService>();

services.AddAutoMapper(typeof(MappingProfile));

var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();
app.Run();