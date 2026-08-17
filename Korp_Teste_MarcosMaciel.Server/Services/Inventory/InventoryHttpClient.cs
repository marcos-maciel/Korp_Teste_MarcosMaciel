using System.Net;
using System.Net.Http.Json;
using Korp_Teste_MarcosMaciel.Server.DTOs;
using Korp_Teste_MarcosMaciel.Server.Exceptions;
using Korp_Teste_MarcosMaciel.Server.Models;
using Microsoft.Extensions.Configuration;

namespace Korp_Teste_MarcosMaciel.Server.Services.Inventory;

public class InventoryHttpClient : IInventoryClient
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;

    public InventoryHttpClient(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _configuration = configuration;
    }

    public async Task<int> GetAvailableStockAsync(int productId, CancellationToken cancellationToken = default)
    {
        if (_configuration.GetValue<bool>("SimulateInventoryServiceFailure"))
        {
            throw new HttpRequestException("O serviço de estoque está temporariamente indisponível. Tente novamente em alguns instantes.");
        }

        var response = await _httpClient.GetAsync($"/api/inventory/products/{productId}", cancellationToken);
        response.EnsureSuccessStatusCode();

        var product = await response.Content.ReadFromJsonAsync<Product>(cancellationToken);
        return product?.Saldo ?? 0;
    }

    public async Task<bool> ReserveStockAsync(int productId, int quantity, CancellationToken cancellationToken = default)
    {
        if (_configuration.GetValue<bool>("SimulateInventoryServiceFailure"))
        {
            throw new HttpRequestException("O serviço de estoque está temporariamente indisponível. Tente novamente em alguns instantes.");
        }

        var payload = new InventoryStockRequest { ProductId = productId, Quantity = quantity };
        var response = await _httpClient.PostAsJsonAsync("/api/inventory/stock/reserve", payload, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync(cancellationToken);

            if (response.StatusCode == HttpStatusCode.Conflict)
            {
                throw new DomainException(error);
            }

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                throw new NotFoundException(error);
            }

            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                throw new InvalidOperationException(error);
            }

            throw new HttpRequestException($"O serviço de estoque está temporariamente indisponível. {error}");
        }

        var result = await response.Content.ReadFromJsonAsync<InventoryStockResponse>(cancellationToken);
        return result?.Success == true;
    }
}
