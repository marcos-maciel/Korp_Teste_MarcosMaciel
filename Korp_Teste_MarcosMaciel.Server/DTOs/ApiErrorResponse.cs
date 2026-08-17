namespace Korp_Teste_MarcosMaciel.Server.DTOs;

public class ApiErrorResponse
{
    public string Message { get; set; } = string.Empty;
    public string? Details { get; set; }
    public DateTime TimestampUtc { get; set; } = DateTime.UtcNow;
}
