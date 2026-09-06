namespace DeliveryApp.WebApi.Compartilhado.Modulos.Estabelecimentos;

public sealed record CadastrarEstabelecimentoRequest(
    string NomeComercial,
    string Documento,
    string Endereco,
    string Telefone,
    string AreaAtendimento,
    TimeOnly HoraAbertura,
    TimeOnly HoraFechamento,
    string Email,
    string Senha
);
public sealed record CadastrarEstabelecimentoResponse(
    Guid EstabelecimentoId,
    string NomeComercial
);
