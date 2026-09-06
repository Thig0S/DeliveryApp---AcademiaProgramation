using DeliveryApp.Aplicacao.Modulos.Estabelecimentos;
using DeliveryApp.WebApi.Compartilhado.Http;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DeliveryApp.WebApi.Compartilhado.Modulos.Estabelecimentos;

[ApiController]
[Route("api/estabelecimentos")]
public class EstabelecimentosController(IMediator mediator) : ControllerBase
{
    [AllowAnonymous]
    [HttpPost("cadastro")]
    [ProducesResponseType<CadastrarEstabelecimentoRequest>(StatusCodes.Status201Created)]
    public async Task<ActionResult<CadastrarEstabelecimentoRequest>> Cadastrar(
        CadastrarEstabelecimentoRequest req,
        CancellationToken cancellationToken)
    {
        var resultado = await mediator.Send(new CadastrarEstabelecimentoCommand(
            req.NomeComercial,
            req.Documento,
            req.Endereco,
            req.Telefone,
            req.AreaAtendimento,
            req.HoraAbertura,
            req.HoraFechamento,
            req.Email,
            req.Senha
        ), cancellationToken);

        if (resultado.IsFailed)
            return this.ProblemDetails(resultado);

        return CreatedAtAction(nameof(string.Empty),
        new { clienteid = resultado.Value },
        new CadastrarEstabelecimentoResponse(
            resultado.Value,
            req.NomeComercial
        ));
    }
}
