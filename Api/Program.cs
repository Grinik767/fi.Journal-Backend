using Api;
using Api.Extensions;
using Api.Middlewares;
using Application.Jobs;
using Application.Services.EmailConfirmations;
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
using Infrastructure.DeleteUserDiffsJob;
using Infrastructure.PasswordHasher;
using Infrastructure.Repositories.EmailConfirmations;
using Infrastructure.Repositories.Groups;
using Infrastructure.Repositories.Tables;
using Infrastructure.Repositories.UserDiffs;
using Infrastructure.Repositories.UserNotifications;
using Infrastructure.Repositories.Users;
using Mailer;
using Microsoft.EntityFrameworkCore;
using Quartz;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
var configuration = builder.Configuration;

services.AddSwaggerGen();
services.AddControllers();

services.AddDbContext<JournalDbContext>(
    options => options.UseNpgsql(configuration.GetConnectionString(nameof(JournalDbContext)))
);

services.AddSingleton<IPasswordHasher, PasswordHasherBCrypt>();

services.AddTransient<ExceptionMiddleware>();

services.AddTransient<IValidator<User>, UserValidator>();
services.AddTransient<IValidator<Group>, GroupValidator>();
services.AddTransient<IValidator<Table>, TableValidator>();
services.AddTransient<IValidator<UserDiff>, UserDiffValidator>();

services.AddScoped<IUsersRepository, UsersRepository>();
services.AddScoped<IGroupsRepository, GroupsRepository>();
services.AddScoped<ITablesRepository, TablesRepository>();
services.AddScoped<IUserDiffsRepository, UserDiffsRepository>();
services.AddScoped<IEmailConfirmationsRepository, EmailConfirmationsRepository>();
services.AddScoped<IUserNotificationRepository, UserNotificationRepository>();

services.AddScoped<IUsersService, UsersService>();
services.AddScoped<IGroupsService, GroupsService>();
services.AddScoped<ITablesService, TablesService>();
services.AddScoped<IUserDiffsService, UserDiffsService>();
services.AddScoped<IEmailConfirmationsService, EmailConfirmationsService>();

services.AddSingleton<IGoogleSheetManager, GoogleSheetManager>(_ =>
    new GoogleSheetManager(configuration.GetConnectionString("GoogleCredentialsPath")!));

services.AddSingleton<IExcelParser, ExcelParser>();
services.AddCors(options =>
{
    var allowedOrigins = configuration.GetSection("Cors:AllowedOrigins").Get<string[]>();

    options.AddPolicy("AllowFrontend",
        corsPolicyBuilder => corsPolicyBuilder
            .WithOrigins(allowedOrigins!)
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials());
});

services.AddQuartz(q =>
{
    q.ScheduleJob<UserDiffsDeleter>(trigger => trigger
        .WithIdentity("deleteDiffsTrigger")
        .WithCronSchedule("0 0 1 * * ?", cron => cron
            .InTimeZone(TimeZoneInfo.FindSystemTimeZoneById("Asia/Yekaterinburg"))
        )
    );
});

services.AddQuartz(q =>
{
    q.ScheduleJob<UpdateTablesJob>(trigger => trigger
        .WithIdentity("UpdateTablesTrigger")
        .WithCronSchedule("0 */4 * * * ?", cron => cron
            .InTimeZone(TimeZoneInfo.FindSystemTimeZoneById("Asia/Yekaterinburg"))
        )
    );
});

services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);

services.Configure<MailerOptions>(configuration.GetSection(nameof(MailerOptions)));
services.AddSingleton<EmailService>();

services.AddAutoMapper(typeof(MappingProfile));

services.AddApiAuthentication(configuration)
    .AddDenyAuthenticatedPolicy()
    .AddSuperAdminPolicy()
    .AddPersonalDataAccessPolicy()
    .AddUserIsGroupMemberPolicy()
    .AddCombinedPolicies();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseMiddleware<ExceptionMiddleware>();

app.UseCors("AllowFrontend");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.Run();