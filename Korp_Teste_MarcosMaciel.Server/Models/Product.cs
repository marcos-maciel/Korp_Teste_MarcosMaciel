namespace Korp_Teste_MarcosMaciel.Server.Models;

public class Product
{
    public int Id { get; set; }
    public string Codigo { get; set; } = string.Empty;
    public string Descricao { get; set; } = string.Empty;
    public int Saldo { get; set; }
    public DateTime CriadoEm { get; set; } = DateTime.UtcNow;
    public DateTime AtualizadoEm { get; set; } = DateTime.UtcNow;

    public void Validar()
    {
        if (string.IsNullOrWhiteSpace(Codigo))
            throw new InvalidOperationException("Código do produto é obrigatório.");

        if (string.IsNullOrWhiteSpace(Descricao))
            throw new InvalidOperationException("Descrição do produto é obrigatória.");

        if (Saldo < 0)
            throw new InvalidOperationException("Saldo do produto não pode ser negativo.");
    }
}
