namespace Korp_Teste_MarcosMaciel.Server.Models;

public class NotaFiscal
{
    public int Id { get; set; }
    public int Numero { get; set; }
    public string Status { get; set; } = "Aberta";
    public List<NotaFiscalItem> Itens { get; set; } = new();
    public DateTime CriadoEm { get; set; } = DateTime.UtcNow;
    public DateTime AtualizadoEm { get; set; } = DateTime.UtcNow;

    public void Validar()
    {
        if (Numero <= 0)
            throw new InvalidOperationException("Número da nota fiscal é obrigatório.");

        if (string.IsNullOrWhiteSpace(Status))
            throw new InvalidOperationException("Status da nota fiscal é obrigatório.");

        if (Itens == null || Itens.Count == 0)
            throw new InvalidOperationException("A nota fiscal deve conter pelo menos um item.");

        if (Itens.Any(x => x.Quantidade <= 0))
            throw new InvalidOperationException("Cada item da nota fiscal deve ter quantidade maior que zero.");
    }
}
