using CShells.AspNetCore.Features;
using CShells.Features;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ModularWebApp.Module.Greetings;

[ShellFeature("Greetings", DisplayName = "Greetings Module")]
public class GreetingsFeature : IWebShellFeature
{
    public void ConfigureServices(IServiceCollection services)
    {
    }

    public void MapEndpoints(IEndpointRouteBuilder endpoints, IHostEnvironment? environment)
    {
        endpoints.MapGet("features/greetings", () => Results.Ok(new
        {
            Message = "Hello from Greetings module",
            Environment = environment?.EnvironmentName
        }));
    }
}
