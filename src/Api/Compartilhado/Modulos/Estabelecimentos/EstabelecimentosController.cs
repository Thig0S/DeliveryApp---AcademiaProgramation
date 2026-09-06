using DeliveryApp.Aplicacao.Compartilhado;
using DeliveryApp.Aplicacao.Modulos.Estabelecimentos;
using DeliveryApp.Aplicacao.Modulos.Estabelecimentos.Util;
using DeliveryApp.Dominio.Compartilhado.Auth;
using DeliveryApp.WebApi.Compartilhado.Http;
using DeliveryApp.WebApi.Compartilhado.Modulos.Clientes;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace DeliveryApp.WebApi.Compartilhado.Modulos.Estabelecimentos;

[ApiController]
[Route("api/estabelecimentos")]
public class EstabelecimentosController(
    IMediator mediator,
    UserManager<IdentityUser<Guid>> userManager,
    SignInManager<IdentityUser<Guid>> signInManager,
    IEmissorDeTokens emissorDeTokens) : ControllerBase
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
    [AllowAnonymous]
    [HttpPost("login")]
    [ProducesResponseType<AutenticarEstabelecimentoResponse>(StatusCodes.Status200OK)]
    public async Task<ActionResult> Autenticar(AutenticarEstabelecimentoCommand command)
    {
        var usuario = await userManager.FindByEmailAsync(command.Email.Trim());

        if (usuario is null)
            return Unauthorized();

        var resultadoAutenticacao = await signInManager.CheckPasswordSignInAsync(
            usuario,
            command.Senha,
            lockoutOnFailure: true);

        if (!resultadoAutenticacao.Succeeded)
            return Unauthorized();

        var classToken = emissorDeTokens.CriarToken(usuario.Id, usuario.Email!, TipoUsuario.Cliente);

        return StatusCode(StatusCodes.Status200OK,
        new AutenticarEstabelecimentoResponse(
            usuario.Id,
            classToken.Token,
            classToken.DataExpiracaoEmUtc
        ));
    }
}
