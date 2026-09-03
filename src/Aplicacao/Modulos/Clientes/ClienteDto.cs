namespace DeliveryApp.Aplicacao.Modulos.Clientes;

public sealed record ClienteDto(Guid Id, string Nome, string Cpf, string Email);
