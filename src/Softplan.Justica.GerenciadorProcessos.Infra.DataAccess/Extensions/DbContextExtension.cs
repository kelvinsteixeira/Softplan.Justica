using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace Softplan.Justica.GerenciadorProcessos.Infra.DataAccess.Extensions
{
    public static class DbContextExtension
    {
        // From : https://entityframeworkcore.com/knowledge-base/42885020/how-to-discard-changes-to-context-in-ef-core
        public static void ResetContextState(this DbContext context)
        {
            context.ChangeTracker.Entries().Where(e => e.Entity != null).ToList().ForEach(e => e.State = EntityState.Detached);
        }
    }
}