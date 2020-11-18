using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Softplan.Justica.GerenciadorProcessos.Domain.Models;

namespace Softplan.Justica.GerenciadorProcessos.Infra.DataAccess.Mappings
{
    public class SituacaoProcessoMapping : IEntityTypeConfiguration<SituacaoProcesso>, IEntityMapping
    {
        public void Configure(EntityTypeBuilder<SituacaoProcesso> builder)
        {
            builder.HasKey(p => p.Id);

            builder.Property(p => p.Nome).IsRequired();

            builder.HasMany(p => p.Processos)
                .WithOne(sp => sp.Situacao);
        }
    }
}