using DeliveryApp.Aplicacao.Modulos.Clientes;
using DeliveryApp.WebApi.Compartilhado.Modulos.Clientes;
using Microsoft.Extensions.DependencyInjection;

namespace DeliveryApp.Aplicacao;

public static class DependencyInjection
{
    public static void AddApplicationServices(
        this IServiceCollection services
    )
    {
        using var serviceProvider = services.BuildServiceProvider();
        services.AddScoped<ObterClientePorIdQueryHanlder>();
        services.AddScoped<CadastrarClienteCommandHandler>();
    }
}
