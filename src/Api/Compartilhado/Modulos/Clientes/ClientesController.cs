using DeliveryApp.Dominio.Compartilhado.Auth;
using DeliveryApp.Dominio.Modulos.Clientes;
using DeliveryApp.Infraestrutura.Orm;
using DeliveryApp.WebApi.Compartilhado.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DeliveryApp.WebApi.Compartilhado.Modulos.Clientes;

[ApiController]
[Route("api/clientes")]
public class ClientesController(
        DeliveryAppDbContext dbContext,
        UserManager<IdentityUser<Guid>> userManager,
        SignInManager<IdentityUser<Guid>> signInManager,
        RoleManager<IdentityRole<Guid>> roleManager,
        JwtProvider jwtProvider
) : ControllerBase
{
    [AllowAnonymous]
    [HttpPost]
    public async Task<ActionResult<AutenticacaoClienteResponse>> Cadastrar(CadastrarClienteRequest req)
    {
        var cliente = new Cliente(Guid.CreateVersion7(), req.Nome, req.Cpf);

        var erros = cliente.Validar();

        if (erros.Count > 0)
        {
            return BadRequest();
        }

        if (await dbContext.Clientes.AnyAsync(registro => registro.Cpf == cliente.Cpf))
        {
            return Conflict();
        }

        var usuario = new IdentityUser<Guid>
        {
            Id = cliente.Id,
            Email = req.Email.Trim(),
            UserName = req.Email.Trim()
        };

        try
        {
            var resultadoUsuario = await userManager.CreateAsync(usuario, req.Senha);

            if (!resultadoUsuario.Succeeded)
                return BadRequest();

            string tipoUsuario = TipoUsuario.Cliente.ToString();

            var resultadoRole = await roleManager.FindByNameAsync(tipoUsuario);

            if (resultadoRole is null)
            {
                await roleManager.CreateAsync(new IdentityRole<Guid>
                {
                    Id = new Guid("01a0651a-a522-7a83-a062-033b797331d0"),
                    Name = TipoUsuario.Cliente.ToString(),
                    NormalizedName = TipoUsuario.Cliente.ToString().ToUpperInvariant(),
                    ConcurrencyStamp = "01a0651d-7402-7053-874c-fe91e0612b5a"
                });
            }
            var resultadoInclusaoRole = await userManager.AddToRoleAsync(usuario, tipoUsuario);

            if (!resultadoInclusaoRole.Succeeded)
            {
                await userManager.DeleteAsync(usuario);

                return StatusCode(500);
            }

            dbContext.Clientes.Add(cliente);

            await dbContext.SaveChangesAsync();

            var jwt = jwtProvider.CriarToken(usuario.Id, usuario.Email!, TipoUsuario.Cliente);

            return StatusCode(StatusCodes.Status201Created,
            new AutenticacaoClienteResponse(
                cliente.Id,
                jwt.AccessToken,
                jwt.DataExpiracaoEmUtc
            ));
        }
        catch (DbUpdateException)
        {
            await userManager.DeleteAsync(usuario);

            return Conflict();
        }
    }
}
