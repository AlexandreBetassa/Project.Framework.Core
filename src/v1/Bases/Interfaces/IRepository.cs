using Microsoft.EntityFrameworkCore.Query;
using Project.Framework.Core.v1.Bases.Entities;
using System.Linq.Expressions;

namespace Project.Framework.Core.v1.Bases.Interfaces
{
    public interface IRepository<T> where T : BaseEntity
    {
        Task UpdateAsync(T entity);

        Task<T?> GetByIdAsync(string id);

        Task<Guid> CreateAsync(T entity);

        Task PatchAsync(string id, Expression<Func<SetPropertyCalls<T>, SetPropertyCalls<T>>> expression);

        Task SaveChangesAsync();
    }
}
