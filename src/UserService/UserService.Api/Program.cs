using NLog;
using NLog.Web;
using UserService.Api.Extensions;
using UserService.Api.Filters;
using UserService.Api.Middlewares;
using UserService.Application.Models;

namespace UserService.Api;

public class Program
{
    public static void Main(string[] args)
    {
        var log = LogManager.GetCurrentClassLogger();
        try
        {
            var builder = WebApplication.CreateBuilder(args);
            log.Info("started building builder");

            builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
            builder.Services.AddProblemDetails();

            builder.Services.AddJwtAuthentication(builder.Configuration);
            builder.Services.AddDatabaseContext(builder.Configuration);

            builder.Services.AddSingleton(TimeProvider.System);
            builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));

            builder.Services.AddApplicationServices();
            builder.Services.AddControllers(options =>
            {
                options.Filters.Add<LogExecutionTimeFilter>();
            });

            builder.Logging.ClearProviders();
            builder.Host.UseNLog();

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerDocumentation();

            var app = builder.Build();
            log.Info("building app");

            app.UseExceptionHandler();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwaggerDocumentation();
            }

            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();
            app.MapHealthChecks("/health");
            
            log.Info("running app");
            app.Run();
        }
        catch (Exception ex)
        {
            log.Fatal(ex, "Application stopped because of the exception");
            throw;
        }
        finally
        {
            LogManager.Shutdown();
        }
    }
}
