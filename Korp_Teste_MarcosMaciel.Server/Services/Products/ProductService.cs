using Korp_Teste_MarcosMaciel.Server.Data.Interfaces;
using Korp_Teste_MarcosMaciel.Server.Exceptions;
using Korp_Teste_MarcosMaciel.Server.Models;
using Korp_Teste_MarcosMaciel.Server.Services.Inventory;

namespace Korp_Teste_MarcosMaciel.Server.Services.Products;

public class ProductService
{
    private readonly IProductRepository _repository;
    private readonly IInventoryClient _inventoryClient;

    public ProductService(IProductRepository repository, IInventoryClient inventoryClient)
    {
        _repository = repository;
        _inventoryClient = inventoryClient;
    }

    public async Task<List<Product>> GetAllAsync()
    {
        return await _repository.GetAllAsync();
    }

    public async Task<Product?> GetByIdAsync(int id)
    {
        return await _repository.GetByIdAsync(id);
    }

    public async Task<Product> CreateAsync(Product product)
    {
        product.Validar();

        if (await _repository.ExistsByCodigoAsync(product.Codigo.Trim()))
        {
            throw new DomainException($"Já existe um produto com o código '{product.Codigo}'.");
        }

        product.Codigo = product.Codigo.Trim();
        product.Descricao = product.Descricao.Trim();
        product.AtualizadoEm = DateTime.UtcNow;

        var created = await _repository.AddAsync(product);
        await _inventoryClient.RegisterProductAsync(created);
        return created;
    }
}
