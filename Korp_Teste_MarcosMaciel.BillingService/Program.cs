using System.Collections.Concurrent;

var invoiceStatuses = new ConcurrentDictionary<int, string>();

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGet("/health", () => Results.Ok(new { status = "healthy", service = "billing" }));

app.MapGet("/api/billing/notes/{id:int}/status", (int id) =>
{
    var status = invoiceStatuses.GetOrAdd(id, "Aberta");
    return Results.Ok(new { noteId = id, status });
});

app.MapPost("/api/billing/notes/{id:int}/status", (int id, UpdateInvoiceStatusRequest request) =>
{
    if (string.IsNullOrWhiteSpace(request.Status))
    {
        return Results.BadRequest(new { message = "Status inválido." });
    }

    var normalized = request.Status.Trim();
    invoiceStatuses[id] = normalized;

    return Results.Ok(new { noteId = id, status = normalized });
});

app.Run();

public record UpdateInvoiceStatusRequest
{
    public string Status { get; init; } = string.Empty;
}
