# modular-web-app
A minimalistic modular reference ASP.NET app that demonstrates shells and dynamic package loading using Nuplane

## What this sample demonstrates

- ASP.NET Core empty-template host with **CShells** + **Nuplane**.
- Multiple shells (Default, Northwind, Fabrikam) with different feature sets.
- Features shipped as **NuGet packages** that are **not statically referenced** by the host.
- Runtime package discovery from `src/ModularWebApp.Host/packages`.
- Manual runtime reload endpoints:
  - `POST /admin/reconcile`
  - `POST /admin/reload-shells`
  - `GET /admin/catalog`

## Projects

- `/src/ModularWebApp.Host` - main host app.
- `/src/ModularWebApp.Module.Greetings` - module package exposing `Greetings` feature.
- `/src/ModularWebApp.Module.Time` - module package exposing `Time` feature.

## Run locally

1. Pack modules to `.nupkg` files into the host package folder:

   ```bash
   dotnet pack /home/runner/work/modular-web-app/modular-web-app/src/ModularWebApp.Module.Greetings/ModularWebApp.Module.Greetings.csproj -c Debug
   dotnet pack /home/runner/work/modular-web-app/modular-web-app/src/ModularWebApp.Module.Time/ModularWebApp.Module.Time.csproj -c Debug
   ```

2. Run the host:

   ```bash
   dotnet run --project /home/runner/work/modular-web-app/modular-web-app/src/ModularWebApp.Host/ModularWebApp.Host.csproj
   ```

3. Trigger runtime load + shell refresh:

   ```bash
   curl -X POST http://localhost:5000/admin/reconcile
   curl -X POST http://localhost:5000/admin/reload-shells
   ```

4. Verify shell-specific feature endpoints:

   ```bash
   curl http://localhost:5000/features/greetings
   curl http://localhost:5000/northwind/features/greetings
   curl http://localhost:5000/northwind/features/time
   curl http://localhost:5000/fabrikam/features/time
   ```
