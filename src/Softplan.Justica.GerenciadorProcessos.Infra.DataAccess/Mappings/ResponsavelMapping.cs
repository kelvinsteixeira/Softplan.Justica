using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Softplan.Justica.GerenciadorProcessos.Domain.Models;

namespace Softplan.Justica.GerenciadorProcessos.Infra.DataAccess.Mappings
{
    public class ResponsavelMapping : IEntityTypeConfiguration<Responsavel>, IEntityMapping
    {
        public void Configure(EntityTypeBuilder<Responsavel> builder)
        {
            builder.HasKey(prop => prop.Id);

            builder.Property(p => p.Cpf).IsRequired().HasMaxLength(11);
            builder.Property(p => p.Email).IsRequired().HasMaxLength(400);
            builder.Property(p => p.Nome).IsRequired().HasMaxLength(150);

            builder.HasIndex(prop => prop.Cpf).IsUnique();
        }
    }
}