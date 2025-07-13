using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Project.Framework.Core.v1.Bases.Entities;
using Project.Framework.Core.v1.Bases.Interfaces;
using System.Linq.Expressions;

namespace Project.Framework.Core.v1.Bases.Repository
{
    public class BaseRepository<T>(DbContext context) : IRepository<T>
        where T : BaseEntity
    {
        public DbContext Context { get; } = context;

        public async Task<T?> GetByIdAsync(int id) =>
            await IncludeAllNavigations(Context.Set<T>())
                .FirstOrDefaultAsync(x => x.Id.Equals(id));

        public async Task<int> CreateAsync(T entity)
        {
            await Context.Set<T>().AddAsync(entity);
            await Context.SaveChangesAsync();

            return entity.Id;
        }

        public async Task PatchAsync(int id, Expression<Func<SetPropertyCalls<T>, SetPropertyCalls<T>>> expression) =>
            await Context.Set<T>()
                .Where(x => x.Id.Equals(id))
                .ExecuteUpdateAsync(expression);

        public async Task UpdateAsync(T entity)
        {
            Context.Set<T>().Update(entity);

            await Context.SaveChangesAsync();
        }

        public async Task SaveChangesAsync() =>
            await Context.SaveChangesAsync();

        private IQueryable<T> IncludeAllNavigations(DbSet<T> dbSet)
        {
            var entityType = Context.Model.FindEntityType(typeof(T));
            var query = dbSet.AsQueryable();

            foreach (var navigation in entityType!.GetNavigations())
            {
                query = query.Include(navigation.Name);
            }

            return query;
        }
    }
}