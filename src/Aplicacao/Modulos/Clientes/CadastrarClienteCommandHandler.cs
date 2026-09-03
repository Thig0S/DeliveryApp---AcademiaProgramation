using DeliveryApp.Aplicacao.Compartilhado;
using DeliveryApp.Aplicacao.Modulos.Clientes;
using DeliveryApp.Dominio.Modulos.Clientes;
using FluentResults;

namespace DeliveryApp.WebApi.Compartilhado.Modulos.Clientes;

public sealed record CadastrarClienteComand(
    Guid Id,
    string Nome,
    string Cpf
);

public sealed class CadastrarClienteCommandHandler(
    IRepositorioCliente repositorioCliente
)
{
    public async Task<Result> Handle(CadastrarClienteComand command)
    {
        var cliente = new Cliente(command.Id, command.Nome, command.Cpf);

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

        await repositorioCliente.CadastrarAsync(cliente);

        return Result.Ok();

    }
}
