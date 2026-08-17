using Korp_Teste_MarcosMaciel.Server.Models;

namespace Korp_Teste_MarcosMaciel.Server.Services.Inventory;

public interface IInventoryClient
{
    Task<bool> ReserveStockAsync(int productId, int quantity, CancellationToken cancellationToken = default);
    Task<int> GetAvailableStockAsync(int productId, CancellationToken cancellationToken = default);
    Task RegisterProductAsync(Product product, CancellationToken cancellationToken = default);
}
