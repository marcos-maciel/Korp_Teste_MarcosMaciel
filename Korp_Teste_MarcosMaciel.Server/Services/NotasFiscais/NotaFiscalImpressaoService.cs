using Korp_Teste_MarcosMaciel.Server.Data;
using Korp_Teste_MarcosMaciel.Server.Exceptions;
using Korp_Teste_MarcosMaciel.Server.Models;
using Korp_Teste_MarcosMaciel.Server.Services.Billing;
using Korp_Teste_MarcosMaciel.Server.Services.Inventory;
using Microsoft.EntityFrameworkCore;

namespace Korp_Teste_MarcosMaciel.Server.Services.NotasFiscais;

public class NotaFiscalImpressaoService
{
    private readonly AppDbContext _context;
    private readonly IInventoryClient _inventoryClient;
    private readonly IBillingClient _billingClient;

    public NotaFiscalImpressaoService(AppDbContext context, IInventoryClient inventoryClient, IBillingClient billingClient)
    {
        _context = context;
        _inventoryClient = inventoryClient;
        _billingClient = billingClient;
    }

    public async Task<NotaFiscal> ImprimirAsync(int notaFiscalId)
    {
        var notaFiscal = await _context.NotasFiscais
            .Include(x => x.Itens)
            .FirstOrDefaultAsync(x => x.Id == notaFiscalId);

        if (notaFiscal is null)
        {
            throw new NotFoundException($"Nota fiscal com id {notaFiscalId} não encontrada.");
        }

        if (string.Equals(notaFiscal.Status, "Fechada", StringComparison.OrdinalIgnoreCase))
        {
            return notaFiscal;
        }

        notaFiscal.Validar();

        if (!string.Equals(notaFiscal.Status, "Aberta", StringComparison.OrdinalIgnoreCase))
        {
            throw new DomainException("Somente notas fiscais com status 'Aberta' podem ser impressas.");
        }

        var billingStatus = await _billingClient.GetStatusAsync(notaFiscalId);
        if (!string.Equals(billingStatus, "Aberta", StringComparison.OrdinalIgnoreCase))
        {
            throw new DomainException("A nota fiscal foi sinalizada como fechada pelo microsserviço de faturamento.");
        }

        foreach (var item in notaFiscal.Itens)
        {
            if (item.Quantidade <= 0)
            {
                throw new DomainException("Cada item da nota fiscal deve ter quantidade maior que zero.");
            }

            var canReserve = await _inventoryClient.ReserveStockAsync(item.ProdutoId, item.Quantidade);
            if (!canReserve)
            {
                throw new DomainException($"Produto com id {item.ProdutoId} sem saldo suficiente para a quantidade informada.");
            }
        }

        notaFiscal.Status = "Fechada";
        notaFiscal.AtualizadoEm = DateTime.UtcNow;

        await _billingClient.SetStatusAsync(notaFiscalId, "Fechada");
        await _context.SaveChangesAsync();
        return notaFiscal;
    }
}
