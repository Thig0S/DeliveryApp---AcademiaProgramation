using DeliveryApp.Aplicacao.Modulos.Clientes;
using DeliveryApp.Dominio.Compartilhado.Auth;
using DeliveryApp.Dominio.Modulos.Clientes;
using DeliveryApp.Infraestrutura.Orm;
using DeliveryApp.WebApi.Compartilhado.Auth;
using DeliveryApp.WebApi.Compartilhado.Http;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DeliveryApp.WebApi.Compartilhado.Modulos.Clientes;

[ApiController]
[Route("api/clientes")]
public class ClientesController(
        UserManager<IdentityUser<Guid>> userManager,
        SignInManager<IdentityUser<Guid>> signInManager,
        IEmissorDeTokens emissorDeTokens ,
        IMediator mediator
) : ControllerBase
{
    [Authorize(Roles = nameof(TipoUsuario.Cliente))]
    [HttpGet("{clienteId:guid}")]
    public async Task<ActionResult<ClienteResponse>> ObterPorId(
        Guid clienteId,
        CancellationToken cancellationToken
    )
    {
        var resultado = await mediator.Send(new ObterClientePorIdQuery(clienteId, cancellationToken));

        if (resultado.IsFailed)
        {
            return this.ProblemDetails(resultado);
        }

        var response = resultado.Value;

        return Ok(new ClienteResponse(response.Id, response.Nome, response.Cpf, response.Email));
    }

    [AllowAnonymous]
    [HttpPost("cadastro")]
    public async Task<ActionResult<CadastrarClienteResponse>> Cadastrar(CadastrarClienteRequest req)
    {
        var id = Guid.CreateVersion7();

        var usuario = new IdentityUser<Guid>
        {
            Id = id,
            Email = req.Email.Trim(),
            UserName = req.Email.Trim()
        };

        try
        {
            var resultadoUsuario = await userManager.CreateAsync(usuario, req.Senha);

            if (!resultadoUsuario.Succeeded)
                return BadRequest();

            string tipoUsuario = TipoUsuario.Cliente.ToString();

            var resultadoInclusaoRole = await userManager.AddToRoleAsync(usuario, tipoUsuario);

            if (!resultadoInclusaoRole.Succeeded)
            {
                await userManager.DeleteAsync(usuario);

                return StatusCode(500);
            }

            await mediator.Send(new CadastrarClienteComand(id, req.Nome, req.Cpf));

            return Created(string.Empty, new CadastrarClienteResponse(
                usuario.Id,
                req.Nome
            ));
        }
        catch (DbUpdateException)
        {
            await userManager.DeleteAsync(usuario);

            return Conflict();
        }
    }
    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<ActionResult> Autenticar(AutenticarClienteRequest req)
    {
        var usuario = await userManager.FindByEmailAsync(req.Email.Trim());

        if (usuario is null)
            return Unauthorized();

        var resultadoAutenticacao = await signInManager.CheckPasswordSignInAsync(
            usuario,
            req.Senha,
            lockoutOnFailure: true);

        if (!resultadoAutenticacao.Succeeded)
            return Unauthorized();

        var classToken = emissorDeTokens.CriarToken(usuario.Id, usuario.Email!, TipoUsuario.Cliente);

        return StatusCode(StatusCodes.Status200OK,
        new AutenticacaoClienteResponse(
            usuario.Id,
            classToken.Token,
            classToken.DataExpiracaoEmUtc
        ));
    }
}
