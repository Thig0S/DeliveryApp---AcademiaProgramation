using DeliveryApp.Dominio.Modulos.Estabelecimento;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DeliveryApp.Infraestrutura.Config;

public class EstabelecimentoConfigurations : IEntityTypeConfiguration<Estabelecimento>
{
    public void Configure(EntityTypeBuilder<Estabelecimento> builder)
    {
        builder.ToTable("TBEstabelecimentos");

        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).ValueGeneratedNever();

        builder.Property(e => e.NomeComercial)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(e => e.Documento)
            .HasMaxLength(14)
            .IsRequired();

        builder.Property(e => e.Endereco)
            .HasMaxLength(250)
            .IsRequired();

        builder.Property(e => e.Telefone)
            .HasMaxLength(11)
            .IsRequired();

        builder.Property(e => e.AreaAtendimento)
            .HasMaxLength(150)
            .IsRequired();

        builder.HasOne<IdentityUser<Guid>>()
            .WithOne()
            .HasForeignKey<Estabelecimento>(e => e.Id)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
