namespace DeliveryApp.Dominio.Compartilhado.Auth;

public sealed record AcessToken(string Token, DateTime DataExpiracaoEmUtc);
public interface IEmissorDeTokens
{
    AcessToken CriarToken(Guid usuarioId, string email, TipoUsuario tipoUsuario);
}
