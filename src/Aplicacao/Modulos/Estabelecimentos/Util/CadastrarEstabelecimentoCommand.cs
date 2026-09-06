using FluentResults;
using MediatR;

namespace DeliveryApp.Aplicacao.Modulos.Estabelecimentos.Util;

public sealed record CadastrarEstabelecimentoCommand(
    string NomeComercial,
    string Documento,
    string Endereco,
    string Telefone,
    string AreaAtendimento,
    TimeOnly HoraAbertura,
    TimeOnly HoraFechamento,
    string Email,
    string Senha
) : IRequest<Result<Guid>>;
