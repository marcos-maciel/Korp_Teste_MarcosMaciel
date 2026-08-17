namespace Korp_Teste_MarcosMaciel.Server.Interfaces;

public interface IInventoryService
{
    Task<List<string>> GetAvailableProductsAsync();
    Task<bool> ReserveStockAsync(int productId, int quantity);
}
