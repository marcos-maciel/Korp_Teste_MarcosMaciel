namespace Korp_Teste_MarcosMaciel.Server.Services.Billing;

public interface IBillingClient
{
    Task<string> GetStatusAsync(int noteId, CancellationToken cancellationToken = default);
    Task<bool> SetStatusAsync(int noteId, string status, CancellationToken cancellationToken = default);
}
