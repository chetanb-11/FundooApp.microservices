namespace ApiGateway;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Routes + clusters are defined in appsettings.json under "ReverseProxy"
        builder.Services.AddReverseProxy()
            .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

        builder.Services.AddHealthChecks();

        var app = builder.Build();

        app.UseHttpsRedirection();

        app.MapGet("/", () => "Fundoo API Gateway");
        app.MapHealthChecks("/health");
        app.MapReverseProxy();

        app.Run();
    }
}
