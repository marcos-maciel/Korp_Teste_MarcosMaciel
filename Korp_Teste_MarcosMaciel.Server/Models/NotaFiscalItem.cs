namespace Korp_Teste_MarcosMaciel.Server.Models;

public class NotaFiscalItem
{
    public int Id { get; set; }
    public int NotaFiscalId { get; set; }
    public int ProdutoId { get; set; }
    public int Quantidade { get; set; }
    public Product? Produto { get; set; }
}
