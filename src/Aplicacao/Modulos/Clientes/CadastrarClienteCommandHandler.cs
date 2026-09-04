using System.Data.Common;
using DeliveryApp.Aplicacao.Compartilhado;
using DeliveryApp.Aplicacao.Modulos.Clientes;
using DeliveryApp.Dominio.Compartilhado.Auth;
using DeliveryApp.Dominio.Modulos.Clientes;
using FluentResults;
using MediatR;

namespace DeliveryApp.WebApi.Compartilhado.Modulos.Clientes;

public sealed record CadastrarClienteComand(
    string Nome,
    string Cpf,
    string Email,
    string Senha
) : IRequest<Result<Guid>>;

public sealed class CadastrarClienteCommandHandler(
    IRepositorioCliente repositorioCliente,
    IGerenciadorDeIdentidade gerenciadorDeIdentidade
) : IRequestHandler<CadastrarClienteComand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(CadastrarClienteComand command, CancellationToken cancellationToken = default)
    {
        var cliente = new Cliente(Guid.CreateVersion7(), command.Nome, command.Cpf);

        var erros = cliente.Validar();

        if (erros.Count > 0)
            return new Error("Cliente Invalido!")
            .WithMetadata(nameof(TipoErro), TipoErro.Validacao);

        var clientes = await repositorioCliente.SelecionarTodosAsync();

        if (clientes.Any(registro => registro.Cpf == cliente.Cpf))
        {
            return Result.Fail(new Error(
                "Já existe um cliente com esse CPF!")
                .WithMetadata(nameof(TipoErro), TipoErro.Conflito)
                );
        }

        try
        {
            UsuarioCadastrado usuario = await gerenciadorDeIdentidade.CadastrarAsync(
                cliente.Id,
                command.Email,
                command.Senha,
                TipoUsuario.Cliente
            );

            await repositorioCliente.CadastrarAsync(cliente, cancellationToken);

            return Result.Ok(cliente.Id);

        }
        catch (ValidacaoDeIdentidadeException excecao)
        {
            return Result.Fail(ErrosDeClientes.ValidacaoDeIdentidade(excecao.Campo, excecao.Message));
        }
        catch (ConflitoDeIdentidadeException excecao)
        {
            return Result.Fail(ErrosDeClientes.ConflitoDeIdentidade(excecao.Message));
        }
        catch (DbException)
        {
            await gerenciadorDeIdentidade.ExcluirAsync(cliente.Id);

            return Result.Fail(ErrosDeClientes.CadastroDuplicado());
        }
    }
}
