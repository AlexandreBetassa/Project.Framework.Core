using Microsoft.EntityFrameworkCore.Query;
using Store.Framework.Core.v1.Bases.Entities;
using System.Linq.Expressions;

namespace Store.Framework.Core.v1.Bases.Interfaces
{
    public interface IRepository<T> where T : BaseEntity
    {
        Task UpdateAsync(T entity);

        Task<T?> GetByIdAsync(int id);

        Task<int> CreateAsync(T entity);

        Task PatchAsync(int id, Expression<Func<SetPropertyCalls<T>, SetPropertyCalls<T>>> expression);

        Task SaveChangesAsync();
    }
}
