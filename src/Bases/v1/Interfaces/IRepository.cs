using Fatec.Store.Framework.Core.Bases.v1.Entities;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace Fatec.Store.Framework.Core.Bases.v1.Interfaces
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
