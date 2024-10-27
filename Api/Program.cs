using Infrastructure;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();

var configuration = builder.Configuration;

builder.Services.AddDbContext<JournalDbContext>(
    options => options.UseNpgsql(configuration.GetConnectionString(nameof(JournalDbContext)))
);


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();
app.Run();