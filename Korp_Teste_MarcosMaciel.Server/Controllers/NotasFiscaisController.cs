using Korp_Teste_MarcosMaciel.Server.DTOs;
using Korp_Teste_MarcosMaciel.Server.Exceptions;
using Korp_Teste_MarcosMaciel.Server.Models;
using Korp_Teste_MarcosMaciel.Server.Services.NotasFiscais;
using Microsoft.AspNetCore.Mvc;

namespace Korp_Teste_MarcosMaciel.Server.Controllers;

[ApiController]
[Route("api/notas-fiscais")]
public class NotasFiscaisController : ControllerBase
{
    private readonly NotaFiscalService _notaFiscalService;

    public NotasFiscaisController(NotaFiscalService notaFiscalService)
    {
        _notaFiscalService = notaFiscalService;
    }

    [HttpGet]
    public async Task<ActionResult<List<NotaFiscal>>> GetAll()
    {
        var notas = await _notaFiscalService.GetAllAsync();
        return Ok(notas);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<NotaFiscal>> GetById(int id)
    {
        var notaFiscal = await _notaFiscalService.GetByIdAsync(id);

        if (notaFiscal is null)
        {
            return NotFound(new ApiErrorResponse
            {
                Message = "Nota fiscal não encontrada.",
                Details = $"Não existe nota fiscal com o id {id}."
            });
        }

        return Ok(notaFiscal);
    }

    [HttpPost]
    public async Task<ActionResult<NotaFiscal>> Create([FromBody] NotaFiscal notaFiscal)
    {
        if (notaFiscal is null)
        {
            return BadRequest(new ApiErrorResponse
            {
                Message = "Dados inválidos.",
                Details = "O corpo da requisição está vazio."
            });
        }

        try
        {
            var created = await _notaFiscalService.CreateAsync(notaFiscal);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }
        catch (ArgumentNullException ex)
        {
            return BadRequest(new ApiErrorResponse
            {
                Message = "Dados inválidos.",
                Details = ex.Message
            });
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
