using System.Collections.Concurrent;
using Korp_Teste_MarcosMaciel.Shared.Dtos;

var products = new ConcurrentDictionary<int, ProductSummaryDto>(new[]
{
    new KeyValuePair<int, ProductSummaryDto>(1, new ProductSummaryDto { Id = 1, Codigo = "P-100", Descricao = "Produto teste", Saldo = 5 }),
    new KeyValuePair<int, ProductSummaryDto>(2, new ProductSummaryDto { Id = 2, Codigo = "P-101", Descricao = "TESTE 2", Saldo = 20 }),
    new KeyValuePair<int, ProductSummaryDto>(3, new ProductSummaryDto { Id = 3, Codigo = "P-900", Descricao = "Produto teste API", Saldo = 19 }),
    new KeyValuePair<int, ProductSummaryDto>(4, new ProductSummaryDto { Id = 4, Codigo = "P-200", Descricao = "Produto 200", Saldo = 26 })
});

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGet("/health", () => Results.Ok(new { status = "healthy", service = "inventory" }));

app.MapGet("/api/inventory/products/{id:int}", (int id) =>
{
    if (!products.TryGetValue(id, out var product))
    {
        return Results.NotFound(new { message = $"Produto {id} não encontrado." });
    }

    return Results.Ok(product);
});

app.MapPost("/api/inventory/products", (ProductStockUpsertRequest request) =>
{
    if (request.Id <= 0)
    {
        return Results.BadRequest(new { message = "Produto inválido." });
    }

    if (request.Saldo < 0)
    {
        return Results.BadRequest(new { message = "Saldo inválido." });
    }

    var product = new ProductSummaryDto
    {
        Id = request.Id,
        Codigo = request.Codigo,
        Descricao = request.Descricao,
        Saldo = request.Saldo
    };

    products[request.Id] = product;

    return Results.Ok(new
    {
        success = true,
        message = "Produto sincronizado no serviço de estoque.",
        productId = request.Id,
        availableQuantity = request.Saldo
    });
});

app.MapPost("/api/inventory/stock/reserve", (ReserveStockRequest request) =>
{
    if (request.ProductId <= 0)
    {
        return Results.BadRequest(new { message = "Produto inválido." });
    }

    if (request.Quantity <= 0)
    {
        return Results.BadRequest(new { message = "Quantidade inválida." });
    }

    if (!products.TryGetValue(request.ProductId, out var product))
    {
        return Results.NotFound(new { message = $"Produto {request.ProductId} não encontrado." });
    }

    if (product.Saldo < request.Quantity)
    {
        return Results.Conflict(new { message = "Saldo insuficiente para a operação." });
    }

    product.Saldo -= request.Quantity;

    return Results.Ok(new
    {
        success = true,
        productId = product.Id,
        requestedQuantity = request.Quantity,
        availableQuantity = product.Saldo,
        message = "Estoque atualizado com sucesso."
    });
});

app.Run();

public record ProductStockUpsertRequest
{
    public int Id { get; init; }
    public string Codigo { get; init; } = string.Empty;
    public string Descricao { get; init; } = string.Empty;
    public int Saldo { get; init; }
}

public record ReserveStockRequest
{
    public int ProductId { get; init; }
    public int Quantity { get; init; }
}
