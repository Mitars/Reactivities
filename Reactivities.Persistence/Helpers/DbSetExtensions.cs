using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Reactivities.Persistence.Helpers
{
    public static class DbSetExtensions
    {
        public static ValueTask<T> FindByIdAsync<T>(this DbSet<T> dbSet, object key, CancellationToken cancellationToken)
            where T : class
            => dbSet.FindAsync(new object[] { key }, cancellationToken);
    }
}