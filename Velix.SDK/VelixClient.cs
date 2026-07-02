using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization;
using Polly;
using Polly.Extensions.Http;
using Velix.SDK.Exceptions;
using Velix.SDK.Modules;

namespace Velix.SDK;

public class VelixClient : IDisposable
{
    private readonly HttpClient _http;
    private readonly VelixClientOptions _options;
    internal static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
    };

    public CheckinModule Checkin { get; }
    public PersonsModule Persons { get; }
    public EventsModule Events { get; }
    public TenantsModule Tenants { get; }

    public VelixClient(VelixClientOptions options)
    {
        _options = options;

        var retryPolicy = HttpPolicyExtensions
            .HandleTransientHttpError()
            .OrResult(r => r.StatusCode == HttpStatusCode.TooManyRequests)
            .WaitAndRetryAsync(options.MaxRetries, attempt =>
                TimeSpan.FromSeconds(Math.Pow(2, attempt)));

        var handler = new PolicyHttpMessageHandler(retryPolicy)
        {
            InnerHandler = new HttpClientHandler()
        };

        _http = new HttpClient(handler)
        {
            BaseAddress = new Uri(options.ApiUrl.TrimEnd('/') + "/"),
            Timeout = options.Timeout,
        };
        _http.DefaultRequestHeaders.Add("x-api-key", options.ApiKey);
        _http.DefaultRequestHeaders.UserAgent.Add(
            new ProductInfoHeaderValue("velix-csharp-sdk", "1.0.0"));

        Checkin = new CheckinModule(this);
        Persons = new PersonsModule(this);
        Events = new EventsModule(this);
        Tenants = new TenantsModule(this);
    }

    internal async Task<T> GetAsync<T>(string path, CancellationToken ct = default)
    {
        using var response = await _http.GetAsync(path, ct);
        return await ReadResponse<T>(response, ct);
    }

    internal async Task<T> PostAsync<T>(string path, object body, CancellationToken ct = default)
    {
        var content = JsonContent.Create(body, options: JsonOptions);
        using var response = await _http.PostAsync(path, content, ct);
        return await ReadResponse<T>(response, ct);
    }

    internal async Task<T> PutAsync<T>(string path, object body, CancellationToken ct = default)
    {
        var content = JsonContent.Create(body, options: JsonOptions);
        using var response = await _http.PutAsync(path, content, ct);
        return await ReadResponse<T>(response, ct);
    }

    internal async Task<T> PatchAsync<T>(string path, object body, CancellationToken ct = default)
    {
        var content = JsonContent.Create(body, options: JsonOptions);
        using var response = await _http.PatchAsync(path, content, ct);
        return await ReadResponse<T>(response, ct);
    }

    internal async Task DeleteAsync(string path, CancellationToken ct = default)
    {
        using var response = await _http.DeleteAsync(path, ct);
        await EnsureSuccess(response, ct);
    }

    private async Task<T> ReadResponse<T>(HttpResponseMessage response, CancellationToken ct)
    {
        await EnsureSuccess(response, ct);
        var json = await response.Content.ReadAsStringAsync(ct);
        // identity-core wraps responses in { data: T }
        using var doc = JsonDocument.Parse(json);
        var dataElement = doc.RootElement.TryGetProperty("data", out var d) ? d : doc.RootElement;
        return JsonSerializer.Deserialize<T>(dataElement.GetRawText(), JsonOptions)
            ?? throw new VelixException("Empty response from server");
    }

    private static async Task EnsureSuccess(HttpResponseMessage response, CancellationToken ct)
    {
        if (response.IsSuccessStatusCode) return;

        var body = await response.Content.ReadAsStringAsync(ct);
        string? message = null;
        string? code = null;
        Exception? parseError = null;

        try
        {
            using var doc = JsonDocument.Parse(body);
            if (doc.RootElement.TryGetProperty("message", out var m)) message = m.GetString();
            if (doc.RootElement.TryGetProperty("error", out var e)) code = e.GetString();
        }
        catch (JsonException ex)
        {
            // Body isn't valid JSON (e.g. plain text or HTML error page from a proxy).
            // Preserve the original exception instead of silently swallowing it.
            parseError = ex;
        }

        if (response.StatusCode == HttpStatusCode.TooManyRequests)
        {
            int? retryAfter = null;
            if (response.Headers.TryGetValues("Retry-After", out var values) &&
                int.TryParse(values.FirstOrDefault(), out var seconds))
                retryAfter = seconds;
            throw new RateLimitException(retryAfter);
        }

        if (response.StatusCode == HttpStatusCode.Unauthorized)
        {
            throw new AuthException(
                message ?? "Authentication failed",
                code);
        }

        var errorMessage = message ?? $"Request failed with status {(int)response.StatusCode}";
        throw parseError is null
            ? new VelixException(errorMessage, (int)response.StatusCode, code)
            : new VelixException(errorMessage, (int)response.StatusCode, code, parseError);
    }

    public void Dispose() => _http.Dispose();
}
