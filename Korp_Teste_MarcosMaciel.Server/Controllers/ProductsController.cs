using Korp_Teste_MarcosMaciel.Server.DTOs;
using Korp_Teste_MarcosMaciel.Server.Exceptions;
using Korp_Teste_MarcosMaciel.Server.Models;
using Korp_Teste_MarcosMaciel.Server.Services.Products;
using Microsoft.AspNetCore.Mvc;

namespace Korp_Teste_MarcosMaciel.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly ProductService _productService;

    public ProductsController(ProductService productService)
    {
        _productService = productService;
    }

    [HttpGet]
    public async Task<ActionResult<List<Product>>> GetAll()
    {
        var products = await _productService.GetAllAsync();
        return Ok(products);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<Product>> GetById(int id)
    {
        var product = await _productService.GetByIdAsync(id);

        if (product is null)
        {
            return NotFound(new ApiErrorResponse
            {
                Message = "Produto não encontrado.",
                Details = $"Não existe produto com o id {id}."
            });
        }

        return Ok(product);
    }

    [HttpPost]
    public async Task<ActionResult<Product>> Create([FromBody] Product product)
    {
        if (product is null)
        {
            return BadRequest(new ApiErrorResponse
            {
                Message = "Dados inválidos.",
                Details = "O corpo da requisição está vazio."
            });
        }

        try
        {
            var created = await _productService.CreateAsync(product);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }
        catch (DomainException ex)
        {
            return Conflict(new ApiErrorResponse
            {
                Message = ex.Message,
                Details = "A operação foi rejeitada pela regra de negócio."
            });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new ApiErrorResponse
            {
                Message = "Dados inválidos.",
                Details = ex.Message
            });
        }
    }
}
