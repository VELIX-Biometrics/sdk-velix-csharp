using Velix.SDK.Models;

namespace Velix.SDK.Modules;

public class CheckinModule(VelixClient client)
{
    public Task<CheckinResult> FacialAsync(
        string tenantSlug,
        string frameBase64,
        CancellationToken ct = default) =>
        client.PostAsync<CheckinResult>(
            $"v1/checkin/{tenantSlug}/identify",
            new { frame = frameBase64, method = "facial" },
            ct);

    public Task<CheckinResult> QrAsync(
        string tenantSlug,
        string qrCode,
        CancellationToken ct = default) =>
        client.PostAsync<CheckinResult>(
            $"v1/checkin/{tenantSlug}/identify",
            new { qr_code = qrCode, method = "qr" },
            ct);

    public Task<CheckinResult> PinAsync(
        string tenantSlug,
        string pin,
        CancellationToken ct = default) =>
        client.PostAsync<CheckinResult>(
            $"v1/checkin/{tenantSlug}/identify",
            new { pin, method = "pin" },
            ct);
}
