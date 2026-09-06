using DeliveryApp.Dominio.Modulos.Estabelecimento;
using DeliveryApp.Infraestrutura.Orm;

namespace DeliveryApp.Infraestrutura.Modulos.Estabelecimentos;

public class RepositorioEstabelecimentoEmOrm(DeliveryAppDbContext dbContext) :
    RepositorioBaseEmOrm<Estabelecimento>(dbContext), IRepositorioEstabelecimento
{
}
