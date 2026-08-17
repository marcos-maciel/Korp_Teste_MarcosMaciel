using System.Net;
using System.Net.Http.Json;
using Korp_Teste_MarcosMaciel.Server.DTOs;

namespace Korp_Teste_MarcosMaciel.Server.Services.Billing;

public class BillingHttpClient : IBillingClient
{
    private readonly HttpClient _httpClient;

    public BillingHttpClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<string> GetStatusAsync(int noteId, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.GetAsync($"/api/billing/notes/{noteId}/status", cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            throw new HttpRequestException("O serviço de faturamento está indisponível no momento.");
        }

        var result = await response.Content.ReadFromJsonAsync<BillingStatusResponse>(cancellationToken: cancellationToken);
        return result?.Status ?? "Aberta";
    }

    public async Task<bool> SetStatusAsync(int noteId, string status, CancellationToken cancellationToken = default)
    {
        var payload = new BillingStatusRequest { Status = status };
        var response = await _httpClient.PostAsJsonAsync($"/api/billing/notes/{noteId}/status", payload, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var message = await response.Content.ReadAsStringAsync(cancellationToken);
            throw new HttpRequestException($"O serviço de faturamento está indisponível no momento. {message}");
        }

        var result = await response.Content.ReadFromJsonAsync<BillingStatusResponse>(cancellationToken: cancellationToken);
        return string.Equals(result?.Status, status, StringComparison.OrdinalIgnoreCase);
    }
}
