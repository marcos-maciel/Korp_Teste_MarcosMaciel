using Korp_Teste_MarcosMaciel.Server.Data.Interfaces;
using Korp_Teste_MarcosMaciel.Server.Exceptions;
using Korp_Teste_MarcosMaciel.Server.Models;

namespace Korp_Teste_MarcosMaciel.Server.Services.Products;

public class ProductService
{
    private readonly IProductRepository _repository;

    public ProductService(IProductRepository repository)
    {
        _repository = repository;
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

        return await _repository.AddAsync(product);
    }
}
