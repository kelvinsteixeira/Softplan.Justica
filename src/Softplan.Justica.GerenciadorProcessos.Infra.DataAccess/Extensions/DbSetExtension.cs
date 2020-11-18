using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace Softplan.Justica.GerenciadorProcessos.Infra.DataAccess.Extensions
{
    public static class DbSetExtension
    {
        public static IQueryable<T> IncludeMany<T>(this DbSet<T> dbSet, params string[] includes) where T : class
        {
            foreach (var include in includes)
            {
                dbSet.Include(include);
            }

            return dbSet;
        }
    }
}