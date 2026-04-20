using CShells.AspNetCore.Features;
using CShells.Features;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ModularWebApp.Module.Time;

[ShellFeature("Time", DisplayName = "Time Module")]
public class TimeFeature : IWebShellFeature
{
    public void ConfigureServices(IServiceCollection services)
    {
    }

    public void MapEndpoints(IEndpointRouteBuilder endpoints, IHostEnvironment? environment)
    {
        endpoints.MapGet("features/time", () => Results.Ok(new
        {
            UtcNow = DateTimeOffset.UtcNow,
            Environment = environment?.EnvironmentName
        }));
    }
}
