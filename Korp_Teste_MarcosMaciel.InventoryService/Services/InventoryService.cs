using Korp_Teste_MarcosMaciel.Shared.Dtos;

namespace Korp_Teste_MarcosMaciel.InventoryService.Services;

public class InventoryService
{
    public Task<IEnumerable<ProductSummaryDto>> GetProductsAsync(CancellationToken cancellationToken = default)
    {
        var products = new[]
        {
            new ProductSummaryDto { Id = 1, Codigo = "P-001", Descricao = "Produto Base", Saldo = 10 },
            new ProductSummaryDto { Id = 2, Codigo = "P-002", Descricao = "Produto Adicional", Saldo = 5 }
        };

        return Task.FromResult<IEnumerable<ProductSummaryDto>>(products);
    }

    public Task<bool> ReserveStockAsync(int productId, int quantity, CancellationToken cancellationToken = default)
    {
        if (productId <= 0 || quantity <= 0)
        {
            return Task.FromResult(false);
        }

        return Task.FromResult(true);
    }
}
