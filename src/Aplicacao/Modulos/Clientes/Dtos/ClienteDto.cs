namespace DeliveryApp.Aplicacao.Modulos.Clientes.Dtos;

public sealed record ClienteDto(Guid Id, string Nome, string Cpf, string Email);
