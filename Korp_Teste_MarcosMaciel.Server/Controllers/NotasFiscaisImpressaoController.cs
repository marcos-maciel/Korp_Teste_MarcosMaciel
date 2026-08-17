using Korp_Teste_MarcosMaciel.Server.DTOs;
using Korp_Teste_MarcosMaciel.Server.Exceptions;
using Korp_Teste_MarcosMaciel.Server.Models;
using Korp_Teste_MarcosMaciel.Server.Services.NotasFiscais;
using Microsoft.AspNetCore.Mvc;

namespace Korp_Teste_MarcosMaciel.Server.Controllers;

[ApiController]
[Route("api/notas-fiscais")]
public class NotasFiscaisImpressaoController : ControllerBase
{
    private readonly NotaFiscalImpressaoService _impressaoService;

    public NotasFiscaisImpressaoController(NotaFiscalImpressaoService impressaoService)
    {
        _impressaoService = impressaoService;
    }

    [HttpPost("{id:int}/imprimir")]
    public async Task<ActionResult<NotaFiscal>> Imprimir(int id)
    {
        try
        {
            var notaFiscal = await _impressaoService.ImprimirAsync(id);
            return Ok(notaFiscal);
        }
        catch (NotFoundException ex)
        {
            return NotFound(new ApiErrorResponse
            {
                Message = ex.Message,
                Details = "A nota fiscal informada não foi localizada."
            });
        }
        catch (DomainException ex)
        {
            return Conflict(new ApiErrorResponse
            {
                Message = ex.Message,
                Details = "A operação de impressão foi rejeitada pela regra de negócio."
            });
        }
        catch (HttpRequestException ex)
        {
            return StatusCode(StatusCodes.Status503ServiceUnavailable, new ApiErrorResponse
            {
                Message = "O serviço de estoque está indisponível no momento.",
                Details = ex.Message,
                TimestampUtc = DateTime.UtcNow
            });
        }
        catch (TimeoutException ex)
        {
            return StatusCode(StatusCodes.Status503ServiceUnavailable, new ApiErrorResponse
            {
                Message = "O serviço de estoque está indisponível no momento.",
                Details = ex.Message,
                TimestampUtc = DateTime.UtcNow
            });
        }
    }
}
