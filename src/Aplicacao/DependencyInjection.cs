using DeliveryApp.Aplicacao.Modulos.Pedidos.Mensageria;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DeliveryApp.Aplicacao;

public static class DependencyInjection
{
    public static void AddApplicationServices(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        services.AddMediatR(config =>
        {
            config.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly);
        });

        var connectionStringRabbitMq = configuration.GetConnectionString("RabbitMq")
            ?? throw new InvalidOperationException("A ConectionString \"RabbitMq\" Não foi configurada!");

        services.AddMassTransit(config =>
        {
            //Configura a injeção dos consumers

            config.AddConsumer<CriarPedidoConsumer>();

            config.UsingRabbitMq((context, rabbitMq) =>
            {
                rabbitMq.Host(new Uri(connectionStringRabbitMq));

                rabbitMq.ReceiveEndpoint("pedidos-criados", endpoit =>
                {
                    endpoit.PrefetchCount = 4; //quantas mensagens o rabbitmq deve carregar adiantado
                    endpoit.ConcurrentMessageLimit = 2; //quantas mensagens ele pode processar em paralelo (ao mesmo tempo) qnds de instancia consumers

                    endpoit.ConfigureConsumer<CriarPedidoConsumer>(context);

                });
            });
        });
        services.Configure<MassTransitHostOptions>(options =>
        {
            options.WaitUntilStarted = true;
            options.StartTimeout = TimeSpan.FromSeconds(30);
        });
    }
}
