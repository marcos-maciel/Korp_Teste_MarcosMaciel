namespace Korp_Teste_MarcosMaciel.BillingService.Services;

public class BillingService
{
    public Task<string> GetStatusAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult("Aberta");
    }

    public Task<int> GetNextNumberAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult(1);
    }
}
