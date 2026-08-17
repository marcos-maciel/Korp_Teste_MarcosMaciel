using Korp_Teste_MarcosMaciel.Server.Data.Interfaces;
using Korp_Teste_MarcosMaciel.Server.Models;
using Microsoft.EntityFrameworkCore;

namespace Korp_Teste_MarcosMaciel.Server.Data;

public class NotaFiscalRepository : INotaFiscalRepository
{
    private readonly AppDbContext _context;

    public NotaFiscalRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<NotaFiscal>> GetAllAsync()
    {
        return await _context.NotasFiscais
            .AsNoTracking()
            .Include(x => x.Itens)
            .ThenInclude(x => x.Produto)
            .OrderBy(x => x.Id)
            .ToListAsync();
    }

    public async Task<NotaFiscal?> GetByIdAsync(int id)
    {
        return await _context.NotasFiscais
            .AsNoTracking()
            .Include(x => x.Itens)
            .ThenInclude(x => x.Produto)
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<NotaFiscal> AddAsync(NotaFiscal notaFiscal)
    {
        _context.NotasFiscais.Add(notaFiscal);
        await _context.SaveChangesAsync();
        return notaFiscal;
    }

    public async Task<int> GetNextNumeroAsync()
    {
        var ultimaNota = await _context.NotasFiscais
            .AsNoTracking()
            .OrderByDescending(x => x.Numero)
            .Select(x => x.Numero)
            .FirstOrDefaultAsync();

        return ultimaNota + 1;
    }

    public async Task<bool> ExistsByNumeroAsync(int numero)
    {
        return await _context.NotasFiscais
            .AnyAsync(x => x.Numero == numero);
    }
}
