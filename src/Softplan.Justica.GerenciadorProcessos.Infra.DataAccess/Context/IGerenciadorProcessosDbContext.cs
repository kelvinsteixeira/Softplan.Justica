using Microsoft.EntityFrameworkCore;
using Softplan.Justica.GerenciadorProcessos.Domain.Models;

namespace Softplan.Justica.GerenciadorProcessos.Infra.DataAccess.Context
{
    public interface IGerenciadorProcessosDbContext
    {
        DbSet<Processo> Processo { get; }

        DbSet<Responsavel> Responsavel { get; }

        DbSet<SituacaoProcesso> SituacaoProcesso { get; }

        void EnsureDatabase();
    }
}