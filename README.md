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

var identify = await client.Checkin.IdentifyAsync(new CheckinIdentifyRequest
{
    ImageBase64 = frameBase64,
});
Console.WriteLine(identify.Matched ? "GRANTED" : "DENIED");
```

## Environment Variables

| Variable | Required | Description |
|----------|----------|-------------|
| `VELIX_API_URL` | Yes | API base URL (`https://api.velixbiometrics.com`) |
| `VELIX_API_KEY` | Yes | API key (`vlx_...`), sent as `x-api-key` (or `Authorization: Bearer vlx_...`) |

## Modules

Real surface only — 6 endpoints under `/v1/api/*`, protected by API key
(`x-api-key`). See `code/lib/lib-velix-contracts/openapi/public-api.yaml`
(task #593) for the authoritative contract.

| Module | Methods | Scope required |
|--------|---------|-----------------|
| `client.Onboarding` | `EnrollAsync()` | `onboarding:write` |
| `client.Checkin` | `IdentifyAsync()` | `checkin:write` |
| `client.Lgpd` | `RequestDeletionAsync()` | `lgpd:write` |
| `client.Me` | `GetAsync()` | `me:read` |
| `client.Events` | `CreateGuestAsync()`, `GetGuestAsync()` | `events:write` / `events:read` |

**Velix Time has no API-key surface yet** — there is no `client.Time` module
and none should be added until the server side exposes one (see the "Velix
Time" note in the spec file above).

| `client.Contexts` | `CreateAsync/GetAsync/ListAsync/UpdateAsync/RemoveAsync`, `AuthorizeAsync`, `ListAuthorizationDecisionsAsync`, `CreateLinkRequestAsync` | BearerAuth |
| `client.Memberships` | `CreateAsync`, `ListByContextAsync`, `ListByIdentityAsync`, `UpdateStatusAsync`, `AddRolesAsync`, `RemoveRolesAsync` | BearerAuth |
| `client.ContextRoles` | `CreateAsync`, `ListAsync`, `LinkPermissionsAsync` | BearerAuth |
| `client.ContextPermissions` | `CreateAsync`, `ListAsync` | BearerAuth |
| `client.AuthorizationTokens` | `ValidateAsync` | BearerAuth |

## Identity Context

```csharp
var context = await client.Contexts.CreateAsync(new { name = "Matriz SP", contextType = "location" });
var decision = await client.Contexts.AuthorizeAsync(contextId, new { identityId, permission = "access:enter" });
var membership = await client.Memberships.CreateAsync(contextId, new { identityId, roleIds = new[] { roleId } });
// context exit (definitive, no grace period)
await client.Memberships.UpdateStatusAsync(membershipId, "revoked");
// cross-tenant link — stays PENDING until the person consents via magic link
await client.Contexts.CreateLinkRequestAsync(contextId, new { identityId });
await client.AuthorizationTokens.ValidateAsync("vat_...");
```

## Onboarding Module

```csharp
var result = await client.Onboarding.EnrollAsync(new OnboardingRequest
{
    Name = "João Silva",
    Email = "joao@company.com",
    ExternalId = "EMP-001",
    Frames = new[] { frame1, frame2, frame3 },
});
// result.PersonId, result.Enrolled, result.FramesProcessed
```

## Checkin Module

```csharp
var identify = await client.Checkin.IdentifyAsync(new CheckinIdentifyRequest
{
    ImageBase64 = frameBase64,
    Liveness = new LivenessBlock
    {
        Token = challengeToken,
        Samples = new[] { new LivenessSample { Action = "center", ImageBase64 = sampleBase64 } },
    },
});
// identify.Matched, identify.PersonId
// NOTE: no liveness score is ever returned by this API — never surface one.
```

## LGPD Module

```csharp
var deletion = await client.Lgpd.RequestDeletionAsync(personId);
// deletion.ProtocolNumber
```

## Me Module

```csharp
var me = await client.Me.GetAsync(personId);
// me.Name, me.Email, me.PhotoUrl
```

## Events Module

```csharp
var guest = await client.Events.CreateGuestAsync(eventId, new CreateGuestRequest
{
    Name = "Maria Souza",
    Email = "maria@company.com",
});

var status = await client.Events.GetGuestAsync(eventId, guest.Id!);
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
        var result = await velix.Checkin.IdentifyAsync(new CheckinIdentifyRequest { ImageBase64 = frame });
        return Ok(new { result.Matched });
    }
}
```

## Error Handling

```csharp
using Velix.SDK.Exceptions;

try
{
    var result = await client.Checkin.IdentifyAsync(new CheckinIdentifyRequest { ImageBase64 = frame });
}
catch (AuthException)          { Console.Error.WriteLine("Invalid API key"); }
catch (RateLimitException e)   { Console.Error.WriteLine($"Rate limit — retry after {e.RetryAfterSeconds}s"); }
catch (VelixException e)       { Console.Error.WriteLine($"HTTP {e.StatusCode}: {e.Message}"); }
```

## HTTP Timeout

Default client timeout is **30000ms (30s)**, per the SDK contract notes in
the spec (checkin payloads with liveness samples can be 6-12MB). Override
via `VelixClientOptions.Timeout`:

```csharp
var client = new VelixClient(new VelixClientOptions
{
    ApiUrl = "...",
    ApiKey = "...",
    Timeout = TimeSpan.FromSeconds(45),
});
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
