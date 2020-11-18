using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Softplan.Justica.GerenciadorProcessos.Domain.Models;

namespace Softplan.Justica.GerenciadorProcessos.Infra.DataAccess.Mappings
{
    public class ProcessoResponsavelMapping : IEntityTypeConfiguration<ProcessoResponsavel>, IEntityMapping
    {
        public void Configure(EntityTypeBuilder<ProcessoResponsavel> builder)
        {
            builder.HasKey(pr => new { pr.ProcessoId, pr.ResponsavelId });

            builder
                .HasOne(pr => pr.Processo)
                .WithMany(p => p.ProcessoResponsaveis)
                .HasForeignKey(pr => pr.ProcessoId);

            builder
                .HasOne(pr => pr.Responsavel)
                .WithMany(p => p.ProcessoResponsaveis)
                .HasForeignKey(pr => pr.ResponsavelId);
        }
    }
}