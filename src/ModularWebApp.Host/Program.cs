using CShells.AspNetCore.Configuration;
using CShells.AspNetCore.Extensions;
using CShells.DependencyInjection;
using CShells.Management;
using ModularWebApp.Host;
using Nuplane;
using Nuplane.Abstractions;
using Nuplane.Loading;
using Nuplane.Loading.Hosting.Builder;
using Nuplane.Reconciliation;
using Nuplane.Reconciliation.Models;
using Nuplane.Sources.Directory.Configuration;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;
var nuplaneConfiguration = configuration.GetSection("Nuplane");

builder.Services.AddNuplane(nuplaneConfiguration, nuplane =>
{
    nuplane.AddDirectoryFeedsFromConfiguration(nuplaneConfiguration);
    nuplane.AutoloadPackages(nuplaneConfiguration.GetSection("Loading"));
});

builder.Services.AddSingleton<NuplaneFeatureAssemblyProvider>();

builder.AddShells(shells => shells
    .WithHostAssemblies()
    .WithAssemblyProvider<NuplaneFeatureAssemblyProvider>()
    .WithConfigurationProvider(configuration)
    .WithWebRouting(options =>
    {
        options.EnablePathRouting = true;
        options.ExcludePaths = ["/admin"];
    }));

var app = builder.Build();

app.MapGet("/", () => Results.Ok(new
{
    Message = "Modular Web App is running. Drop module .nupkg files into src/ModularWebApp.Host/packages and call POST /admin/reconcile then POST /admin/reload-shells."
}));

app.MapGet("/admin/catalog", async (IActivePackageCatalog activePackageCatalog, CancellationToken cancellationToken) =>
{
    var snapshot = await activePackageCatalog.GetActivePackagesAsync(cancellationToken);

    return Results.Ok(snapshot.Packages.Select(x => new
    {
        x.PackageId,
        x.Version,
        x.FeedName,
        x.SourceName
    }));
});

app.MapPost("/admin/reconcile", async (IReconciliationService reconciliationService, CancellationToken cancellationToken) =>
{
    var trigger = new ReconciliationTrigger(TriggerType.Manual, "admin/reconcile");
    var result = await reconciliationService.TriggerAsync(trigger, cancellationToken);
    return Results.Ok(result);
});

app.MapPost("/admin/reload-shells", (IShellManager shellManager, ILoggerFactory loggerFactory) =>
{
    _ = Task.Run(async () =>
    {
        try
        {
            await shellManager.ReloadAllShellsAsync(CancellationToken.None);
        }
        catch (Exception ex)
        {
            loggerFactory.CreateLogger("ShellReload").LogError(ex, "Shell reload failed.");
        }
    });

    return Results.Ok(new { Message = "Shell reload requested." });
});

app.MapShells();
app.Run();
