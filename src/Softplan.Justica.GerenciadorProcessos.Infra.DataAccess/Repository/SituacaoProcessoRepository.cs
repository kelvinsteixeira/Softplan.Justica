using System.Linq;
using Softplan.Justica.GerenciadorProcessos.Domain.Models;
using Softplan.Justica.GerenciadorProcessos.Domain.Repository;
using Softplan.Justica.GerenciadorProcessos.Infra.DataAccess.Context;

namespace Softplan.Justica.GerenciadorProcessos.Infra.DataAccess.Repository
{
    public class SituacaoProcessoRepository : ISituacaoProcessoRepository
    {
        private readonly IGerenciadorProcessosDbContext context;

        public SituacaoProcessoRepository(IGerenciadorProcessosDbContext context)
        {
            this.context = context;
        }

        public SituacaoProcesso ObterPorId(int id)
        {
            return this.context.SituacaoProcesso.FirstOrDefault(sp => sp.Id == id);
        }
    }
}