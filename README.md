# Velix.SDK — C# / .NET SDK ![version](https://img.shields.io/badge/version-0.1.0--alpha.1-orange)

> ⚠️ **Alpha / pre-release.** This SDK targets a public API surface that does not yet fully exist on the VELIX backend (see internal task #593). Endpoints and auth may not work against production. Do not use in production integrations yet.

Official .NET SDK for the VELIX Biometrics platform — facial access control B2B SaaS.

## Requirements

- .NET 8+ (C# 12)

## Installation

```bash
dotnet add package Velix.SDK
```

Or via NuGet Package Manager:
```powershell
Install-Package Velix.SDK
```

## Quick Start

```csharp
using Velix.SDK;

var client = new VelixClient(new VelixClientOptions
{
    ApiUrl = Environment.GetEnvironmentVariable("VELIX_API_URL")!,
    ApiKey = Environment.GetEnvironmentVariable("VELIX_API_KEY")!,
});

var result = await client.Checkin.FacialAsync("tenant-slug", frameBase64);
Console.WriteLine(result.Passed ? "GRANTED" : "DENIED");
```

## Environment Variables

| Variable | Required | Description |
|----------|----------|-------------|
| `VELIX_API_URL` | Yes | API base URL (`https://api.velixbiometrics.com`) |
| `VELIX_API_KEY` | Yes | Tenant API key (`vx_live_...` or `vx_sandbox_...`) |

## Modules

| Module | Methods |
|--------|---------|
| `client.Checkin` | `FacialAsync()`, `QrAsync()`, `PinAsync()`, `GetHistoryAsync()` |
| `client.Persons` | `ListAsync()`, `GetAsync()`, `CreateAsync()`, `UpdateAsync()`, `DeleteAsync()`, `EnrollAsync()` |
| `client.Events` | `ListAsync()`, `GetAsync()`, `CreateAsync()`, `ConfigureAsync()` |
| `client.Tenants` | `GetMeAsync()`, `UpdateSettingsAsync()` |

## Checkin Module

```csharp
// Facial identification (base64 JPEG frame)
var result = await client.Checkin.FacialAsync("tenant-slug", frameBase64);
// result.Passed == true
// result.PersonId == "uuid"
// result.PersonName == "João Silva"

// QR code checkin
var result = await client.Checkin.QrAsync("tenant-slug", qrToken);

// PIN checkin
var result = await client.Checkin.PinAsync("tenant-slug", pin);

// Paginated history
var history = await client.Checkin.GetHistoryAsync("tenant-slug", page: 1, pageSize: 20);
```

## Persons Module

```csharp
// List with optional search
var list = await client.Persons.ListAsync(page: 1, pageSize: 20, search: "João");

// Get by ID
var person = await client.Persons.GetAsync("uuid");

// Create
var created = await client.Persons.CreateAsync(new CreatePersonRequest(
    Name: "João Silva",
    Email: "joao@company.com",
    ExternalId: "EMP-001"
));

// Update
await client.Persons.UpdateAsync("uuid", new UpdatePersonRequest(Name: "João B. Silva"));

// Enroll biometrics (minimum 3 base64 frames)
await client.Persons.EnrollAsync("uuid", new[] { frame1, frame2, frame3 });

// Delete
await client.Persons.DeleteAsync("uuid");
```

## Events Module

```csharp
var list    = await client.Events.ListAsync(page: 1, pageSize: 20);
var ev      = await client.Events.GetAsync("uuid");
var created = await client.Events.CreateAsync(new CreateEventRequest(Name: "Conference 2026"));
await client.Events.ConfigureAsync("uuid", new EventConfig(CheckInOpen: true));
```

## Tenants Module

```csharp
var tenant = await client.Tenants.GetMeAsync();
await client.Tenants.UpdateSettingsAsync(new TenantSettings(RequireLiveness: true));
```

## Dependency Injection (ASP.NET Core)

```csharp
// Program.cs
builder.Services.AddVelixSDK(options =>
{
    options.ApiUrl = builder.Configuration["Velix:ApiUrl"]!;
    options.ApiKey = builder.Configuration["Velix:ApiKey"]!;
});

// Controller
public class AccessController(VelixClient velix) : ControllerBase
{
    [HttpPost("checkin")]
    public async Task<IActionResult> Checkin([FromBody] string frame)
    {
        var result = await velix.Checkin.FacialAsync("tenant-slug", frame);
        return Ok(new { result.Passed });
    }
}
```

## Error Handling

```csharp
using Velix.SDK.Exceptions;

try
{
    var result = await client.Checkin.FacialAsync("slug", frame);
}
catch (AuthException)          { Console.Error.WriteLine("Invalid API key"); }
catch (BiometricException e)   { Console.Error.WriteLine($"Biometric: {e.Message}"); }
catch (RateLimitException e)   { Console.Error.WriteLine($"Rate limit — retry after {e.RetryAfter}ms"); }
catch (VelixException e)       { Console.Error.WriteLine($"HTTP {e.StatusCode}: {e.Message}"); }
```

## Running Tests

```bash
dotnet test
dotnet test --logger "console;verbosity=detailed"
```

## Local Development

```bash
dotnet build
dotnet test
dotnet pack    # create .nupkg
```

## Get an API Key

Access the dashboard at **velixbiometrics.com** → Settings → API Keys → New Key.
