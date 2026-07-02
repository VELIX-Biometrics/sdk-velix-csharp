using Velix.SDK;
using Velix.SDK.Models;

const string Img = "iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAQAAAC1HAwCAAAAC0lEQVR42mNk+A8AAQUBAScY42YAAAAASUVORK5CYII=";

void Result(string step, bool ok, string detail) =>
    Console.WriteLine($"RESULT:csharp:{step}:{(ok ? "PASS" : "FAIL")}:{detail}");

bool Reachable(string msg)
{
    var m = msg.ToLowerInvariant();
    foreach (var s in new[] { "route not found", "no route", "401", "403" })
        if (m.Contains(s)) return false;
    return true;
}

var client = new VelixClient(new VelixClientOptions
{
    ApiUrl = Environment.GetEnvironmentVariable("API_BASE_URL")!,
    ApiKey = Environment.GetEnvironmentVariable("VELIX_API_KEY")!,
});

string? personId = null;
try
{
    var r = await client.Onboarding.EnrollAsync(new OnboardingRequest { Name = "Smoke Test C#", Frames = new List<string> { Img, Img, Img } });
    personId = r.PersonId;
    Result("onboarding", personId != null, $"person_id={personId}");
}
catch (Exception e) { Result("onboarding", false, e.Message); }

try
{
    var r = await client.Checkin.IdentifyAsync(new CheckinIdentifyRequest { ImageBase64 = Img });
    Result("checkin", true, $"matched={r.Matched}");
}
catch (Exception e) { Result("checkin", false, e.Message); }

if (personId != null)
{
    try { await client.Lgpd.RequestDeletionAsync(personId); Result("lgpd", true, "deletion-request ok"); }
    catch (Exception e) { Result("lgpd", false, e.Message); }

    try { await client.Me.GetAsync(personId); Result("me", true, "got response"); }
    catch (Exception e) { Result("me", false, e.Message); }
}

var dummy = "00000000-0000-0000-0000-000000000000";
try
{
    await client.Events.CreateGuestAsync(dummy, new CreateGuestRequest { Name = "Guest Smoke", Email = "guest@smoke.test" });
    Result("events_create", true, "endpoint reachable");
}
catch (Exception e) { Result("events_create", Reachable(e.Message), e.Message); }

try
{
    await client.Events.GetGuestAsync(dummy, dummy);
    Result("events_get", true, "endpoint reachable");
}
catch (Exception e) { Result("events_get", Reachable(e.Message), e.Message); }
