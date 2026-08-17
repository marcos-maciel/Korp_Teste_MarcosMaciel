namespace Korp_Teste_MarcosMaciel.Server.DTOs;

public class InventoryStockResponse
{
    public bool Success { get; set; }
    public int ProductId { get; set; }
    public int RequestedQuantity { get; set; }
    public int AvailableQuantity { get; set; }
    public string Message { get; set; } = string.Empty;
}
