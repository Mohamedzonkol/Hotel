using System.Linq.Expressions;

namespace Hotel.Application.Common.InterFaces
{
    public interface IRepository<T> where T : class
    {
        IEnumerable<T> GetAllAsync(Expression<Func<T, bool>>? filter = null, string? includeProperty = null);
        Task<T> GetAsync(Expression<Func<T, bool>> filter, string? includeProperty = null);
        Task AddAsync(T entity);
        Task<bool> AnyAsync(Expression<Func<T, bool>> filter);
        Task RemoveAsync(T entity);
    }
}
