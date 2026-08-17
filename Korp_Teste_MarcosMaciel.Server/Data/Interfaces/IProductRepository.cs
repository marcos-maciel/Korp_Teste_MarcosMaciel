using Korp_Teste_MarcosMaciel.Server.Models;

namespace Korp_Teste_MarcosMaciel.Server.Data.Interfaces;

public interface IProductRepository
{
    Task<List<Product>> GetAllAsync();
    Task<Product?> GetByIdAsync(int id);
    Task<Product> AddAsync(Product product);
    Task<bool> ExistsByCodigoAsync(string codigo);
}
