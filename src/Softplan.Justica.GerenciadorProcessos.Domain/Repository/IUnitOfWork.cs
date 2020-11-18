using System.Threading.Tasks;

namespace Softplan.Justica.GerenciadorProcessos.Domain.Repository
{
    public interface IUnitOfWork
    {
        Task SaveChangesAsync();

        void UndoChanges();
    }
}