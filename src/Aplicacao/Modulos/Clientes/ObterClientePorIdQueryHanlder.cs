using DeliveryApp.Aplicacao.Compartilhado;
using DeliveryApp.Dominio.Compartilhado.Auth;
using DeliveryApp.Dominio.Modulos.Clientes;
using FluentResults;

namespace DeliveryApp.Aplicacao.Modulos.Clientes;

public sealed record ObterClientePorIdQuery(Guid ClienteId);

public class ObterClientePorIdQueryHanlder(
    IRepositorioCliente repositorioCliente
    , IProvedorDeUsuario provedorDeUsuario)
{
    public async Task<Result<ClienteDto>> Handle(ObterClientePorIdQuery query)
    {
        if (query.ClienteId != provedorDeUsuario.Id)
        {
            return Result.Fail<ClienteDto>(new Error(
                "Um cliente pode acessar apenas suas próprias informações!")
                .WithMetadata(nameof(TipoErro), TipoErro.NaoAutorizado)
                );
        }

        var cliente = await repositorioCliente.SelecionarPorIdAsync(query.ClienteId);

        if (cliente is null)
            return Result.Fail<ClienteDto>(new Error(
                "O cliente com este ID não foi encontrado.")
                .WithMetadata(nameof(TipoErro), TipoErro.NaoAutorizado)
                );

        return Result.Ok(new ClienteDto(cliente.Id, cliente.Nome, cliente.Cpf, provedorDeUsuario.Email!));
    }
}
