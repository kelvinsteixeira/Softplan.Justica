using System.Threading.Tasks;
using Softplan.Justica.GerenciadorProcessos.Domain.Repository;
using Softplan.Justica.GerenciadorProcessos.Infra.DataAccess.Context;
using Softplan.Justica.GerenciadorProcessos.Infra.DataAccess.Extensions;

namespace Softplan.Justica.GerenciadorProcessos.Infra.DataAccess.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly GerenciadorProcessosDbContext context;

        public UnitOfWork(GerenciadorProcessosDbContext context)
        {
            this.context = context;
        }

        public Task SaveChangesAsync()
        {
            return this.context.SaveChangesAsync();
        }

        public void UndoChanges()
        {
            this.context.ResetContextState();
        }
    }
}