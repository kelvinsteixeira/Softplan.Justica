using Microsoft.EntityFrameworkCore;
using Softplan.Justica.GerenciadorProcessos.Domain.Models;

namespace Softplan.Justica.GerenciadorProcessos.Infra.DataAccess.Extensions
{
    public static class ModelBuilderExtension
    {
        public static void SeedData(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<SituacaoProcesso>().HasData(
                new SituacaoProcesso { Id = 1, Nome = "Em Andamento", Finalizado = false },
                new SituacaoProcesso { Id = 2, Nome = "Desmembrado", Finalizado = false },
                new SituacaoProcesso { Id = 3, Nome = "Em Recurso", Finalizado = false },
                new SituacaoProcesso { Id = 4, Nome = "Finalizado", Finalizado = true },
                new SituacaoProcesso { Id = 5, Nome = "Arquivado", Finalizado = true });
        }
    }
}