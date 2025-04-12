using Fatec.Store.Framework.Core.Bases.v1.Entities;
using Fatec.Store.Framework.Core.Bases.v1.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace Fatec.Store.Framework.Core.Bases.v1.Repository
{
    public class BaseRepository<T>(DbContext context) : IRepository<T>
        where T : BaseEntity
    {
        public DbContext Context { get; } = context;

        public async Task<T?> GetByIdAsync(int id) =>
            await Context.Set<T>()
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id.Equals(id));

        public async Task CreateAsync(T entity) => await Context.Set<T>().AddAsync(entity);

        public async Task PatchAsync(int id, Expression<Func<SetPropertyCalls<T>, SetPropertyCalls<T>>> expression) =>
            await Context.Set<T>()
                .Where(x => x.Id.Equals(id))
                .ExecuteUpdateAsync(expression);

        public async Task Updateasync(T entity) =>
            Context.Set<T>().Update(entity);

        public async Task SaveChangesAsync() =>
            await Context.SaveChangesAsync();
    }
}