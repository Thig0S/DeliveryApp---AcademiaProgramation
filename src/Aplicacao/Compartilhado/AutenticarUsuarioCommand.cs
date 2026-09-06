using FluentResults;
using MediatR;

namespace DeliveryApp.Aplicacao.Compartilhado;


public sealed record AutenticarClienteCommand(
    string Email,
    string Senha
) : IRequest<Result<AccessTokenDoUsuarioDto>>;

public sealed record AutenticarEstabelecimentoCommand(
    string Email,
    string Senha
) : IRequest<Result<AccessTokenDoUsuarioDto>>;

public sealed record AccessTokenDoUsuarioDto(
    Guid UsuarioId,
    string Token,
    DateTime DataExpiracaoEmUtc
);
public sealed record AutenticarEstabelecimentoRequest(string Email, string Senha);

public sealed record AutenticarEstabelecimentoResponse(
    Guid EstabelecimentoId,
    string AccessToken,
    DateTime DataExpiracaoEmUtc

);