using Korp_Teste_MarcosMaciel.Server.Models;

namespace Korp_Teste_MarcosMaciel.Server.Data.Interfaces;

public interface INotaFiscalRepository
{
    Task<List<NotaFiscal>> GetAllAsync();
    Task<NotaFiscal?> GetByIdAsync(int id);
    Task<NotaFiscal> AddAsync(NotaFiscal notaFiscal);
    Task<int> GetNextNumeroAsync();
    Task<bool> ExistsByNumeroAsync(int numero);
}
