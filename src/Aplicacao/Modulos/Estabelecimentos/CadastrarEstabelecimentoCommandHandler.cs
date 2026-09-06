using DeliveryApp.Aplicacao.Modulos.Estabelecimentos.Util;
using DeliveryApp.Dominio.Compartilhado.Auth;
using DeliveryApp.Dominio.Modulos.Estabelecimento;
using FluentResults;
using MediatR;

namespace DeliveryApp.Aplicacao.Modulos.Estabelecimentos;

public class CadastrarEstabelecimentoCommandHandler(IGerenciadorDeIdentidade gerenciadorDeIdentidade,
    IRepositorioEstabelecimento repositorioEstabelecimento) :

    IRequestHandler<CadastrarEstabelecimentoCommand,
    Result<Guid>>
{
    public async Task<Result<Guid>> Handle(
        CadastrarEstabelecimentoCommand command,
        CancellationToken cancellationToken)
    {
        var estabelecimento = new Estabelecimento(
            Guid.CreateVersion7(),
            command.NomeComercial,
            command.Documento,
            command.Endereco,
            command.Telefone,
            command.AreaAtendimento,
            command.HoraAbertura,
            command.HoraFechamento
            );

        var erros = estabelecimento.Validar();

        if (erros.Count > 0)
            return Result.Fail(ErrosDeEstabelecimento.Validacao(erros));

        try
        {
            UsuarioDto usuario = await gerenciadorDeIdentidade.CadastrarAsync(
                estabelecimento.Id,
                command.Email,
                command.Senha,
                TipoUsuario.Estabelecimento
            );

            await repositorioEstabelecimento.CadastrarAsync(estabelecimento, cancellationToken);

            return Result.Ok(estabelecimento.Id);
        }
        catch (ValidacaoDeIdentidadeException ex)
        {
            return Result.Fail(ErrosDeEstabelecimento.ValidacaoDeIdentidade(ex.Campo, ex.Message));
        }
        catch (ConflitoDeIdentidadeException ex)
        {
            return Result.Fail(ErrosDeEstabelecimento.ConflitoDeIdentidade(ex.Message));
        }
        catch (ConflitoDePersistenciaException ex)
        {
            await gerenciadorDeIdentidade.ExcluirAsync(estabelecimento.Id);

            return Result.Fail(ErrosDeEstabelecimento.ConflitoDeIdentidade(ex.Message));
        }
    }
}