using Korp_Teste_MarcosMaciel.Server.Data.Interfaces;
using Korp_Teste_MarcosMaciel.Server.Models;
using Microsoft.EntityFrameworkCore;

namespace Korp_Teste_MarcosMaciel.Server.Data;

public class ProductRepository : IProductRepository
{
    private readonly AppDbContext _context;

    public ProductRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Product>> GetAllAsync()
    {
        return await _context.Products
            .AsNoTracking()
            .OrderBy(x => x.Id)
            .ToListAsync();
    }

    public async Task<Product?> GetByIdAsync(int id)
    {
        return await _context.Products
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<Product> AddAsync(Product product)
    {
        _context.Products.Add(product);
        await _context.SaveChangesAsync();
        return product;
    }

    public async Task<bool> ExistsByCodigoAsync(string codigo)
    {
        return await _context.Products
            .AnyAsync(x => x.Codigo == codigo);
    }
}
