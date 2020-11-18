using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Softplan.Justica.GerenciadorProcessos.Domain.Models;

namespace Softplan.Justica.GerenciadorProcessos.Infra.DataAccess.Mappings
{
    public class ProcessoMapping : IEntityTypeConfiguration<Processo>, IEntityMapping
    {
        public void Configure(EntityTypeBuilder<Processo> builder)
        {
            builder.HasKey(p => p.Id);

            builder.Property(p => p.Descricao).HasMaxLength(1000);
            builder.Property(p => p.NumeroProcesso).HasMaxLength(20).IsRequired();
            builder.Property(p => p.PastaFisicaCliente).HasMaxLength(50);

            builder.HasIndex(p => p.NumeroProcesso).IsUnique();

            builder.HasOne(p => p.Situacao)
                .WithMany(sp => sp.Processos)
                .IsRequired();
        }
    }
}