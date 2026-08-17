using Korp_Teste_MarcosMaciel.Server.Data.Interfaces;
using Korp_Teste_MarcosMaciel.Server.Exceptions;
using Korp_Teste_MarcosMaciel.Server.Models;

namespace Korp_Teste_MarcosMaciel.Server.Services.NotasFiscais;

public class NotaFiscalService
{
    private readonly INotaFiscalRepository _repository;
    private readonly IProductRepository _productRepository;

    public NotaFiscalService(INotaFiscalRepository repository, IProductRepository productRepository)
    {
        _repository = repository;
        _productRepository = productRepository;
    }

    public async Task<List<NotaFiscal>> GetAllAsync()
    {
        return await _repository.GetAllAsync();
    }

    public async Task<NotaFiscal?> GetByIdAsync(int id)
    {
        return await _repository.GetByIdAsync(id);
    }

    public async Task<int> GetNextNumeroAsync()
    {
        return await _repository.GetNextNumeroAsync();
    }

    public async Task<NotaFiscal> CreateAsync(NotaFiscal notaFiscal)
    {
        if (notaFiscal is null)
        {
            throw new ArgumentNullException(nameof(notaFiscal));
        }

        if (notaFiscal.Itens == null || notaFiscal.Itens.Count == 0)
        {
            throw new DomainException("A nota fiscal deve conter pelo menos um item.");
        }

        notaFiscal.Status = string.IsNullOrWhiteSpace(notaFiscal.Status) ? "Aberta" : notaFiscal.Status.Trim();

        if (!string.Equals(notaFiscal.Status, "Aberta", StringComparison.OrdinalIgnoreCase))
        {
            throw new DomainException("A nota fiscal deve iniciar com o status 'Aberta'.");
        }

        notaFiscal.Numero = await _repository.GetNextNumeroAsync();
        notaFiscal.CriadoEm = DateTime.UtcNow;
        notaFiscal.AtualizadoEm = DateTime.UtcNow;

        foreach (var item in notaFiscal.Itens)
        {
            if (item is null)
            {
                throw new DomainException("Não é possível incluir itens nulos na nota fiscal.");
            }

            if (item.Quantidade <= 0)
            {
                throw new DomainException("Cada item da nota fiscal deve ter quantidade maior que zero.");
            }

            var produto = await _productRepository.GetByIdAsync(item.ProdutoId);
            if (produto is null)
            {
                throw new DomainException($"Produto com id {item.ProdutoId} não encontrado.");
            }
        }

        notaFiscal.Validar();

        if (await _repository.ExistsByNumeroAsync(notaFiscal.Numero))
        {
            throw new DomainException($"Já existe uma nota fiscal com o número {notaFiscal.Numero}.");
        }

        return await _repository.AddAsync(notaFiscal);
    }
}
