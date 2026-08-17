using Korp_Teste_MarcosMaciel.Server.Data;
using Korp_Teste_MarcosMaciel.Server.DTOs;
using Korp_Teste_MarcosMaciel.Server.Exceptions;
using Korp_Teste_MarcosMaciel.Server.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Korp_Teste_MarcosMaciel.Server.Controllers;

[ApiController]
[Route("api/inventory")]
public class InventoryController : ControllerBase
{
    private readonly AppDbContext _context;

    public InventoryController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet("products/{id:int}")]
    public async Task<ActionResult<Product>> GetProduct(int id)
    {
        var product = await _context.Products
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id);

        if (product is null)
        {
            throw new NotFoundException($"Produto com id {id} não encontrado.");
        }

        return Ok(product);
    }

    [HttpPost("stock/check")]
    public async Task<ActionResult<InventoryStockResponse>> CheckStock([FromBody] InventoryStockRequest request)
    {
        if (request is null || request.ProductId <= 0 || request.Quantity <= 0)
        {
            return BadRequest(new InventoryStockResponse
            {
                Success = false,
                Message = "Produto e quantidade devem ser válidos."
            });
        }

        var product = await _context.Products
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == request.ProductId);

        if (product is null)
        {
            throw new NotFoundException($"Produto com id {request.ProductId} não encontrado.");
        }

        var hasStock = product.Saldo >= request.Quantity;

        return Ok(new InventoryStockResponse
        {
            Success = hasStock,
            ProductId = product.Id,
            RequestedQuantity = request.Quantity,
            AvailableQuantity = product.Saldo,
            Message = hasStock
                ? "Estoque suficiente para a operação."
                : $"Saldo insuficiente. Disponível: {product.Saldo}, solicitado: {request.Quantity}."
        });
    }

    [HttpPost("stock/reserve")]
    public async Task<ActionResult<InventoryStockResponse>> ReserveStock([FromBody] InventoryStockRequest request)
    {
        if (request is null || request.ProductId <= 0 || request.Quantity <= 0)
        {
            return BadRequest(new InventoryStockResponse
            {
                Success = false,
                Message = "Produto e quantidade devem ser válidos."
            });
        }

        using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            await _context.Database.ExecuteSqlInterpolatedAsync($"SELECT [Id] FROM [Products] WITH (UPDLOCK, ROWLOCK) WHERE [Id] = {request.ProductId}");

            var product = await _context.Products
                .FirstOrDefaultAsync(x => x.Id == request.ProductId);

            if (product is null)
            {
                throw new NotFoundException($"Produto com id {request.ProductId} não encontrado.");
            }

            if (product.Saldo < request.Quantity)
            {
                throw new DomainException($"Saldo insuficiente para o produto '{product.Descricao}'. Disponível: {product.Saldo}, solicitado: {request.Quantity}.");
            }

            product.Saldo -= request.Quantity;
            product.AtualizadoEm = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return Ok(new InventoryStockResponse
            {
                Success = true,
                ProductId = product.Id,
                RequestedQuantity = request.Quantity,
                AvailableQuantity = product.Saldo,
                Message = "Estoque atualizado com sucesso."
            });
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
}
