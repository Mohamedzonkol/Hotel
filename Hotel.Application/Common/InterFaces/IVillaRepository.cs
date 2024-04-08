using Hotel.Domain.Entities;
using System.Linq.Expressions;

namespace Hotel.Application.Common.InterFaces
{
    public interface IVillaRepository
    {
        Task<IEnumerable<Villa>> GetAllAsync(Expression<Func<Villa, bool>>? filter = null, string? includeProperty = null);
        Task<IQueryable<Villa>> GetAsync(Expression<Func<Villa, bool>> filter, string? includeProperty = null);
        Task AddAsync(Villa villa);
        Task UpdateAsync(Villa villa);
        Task RemoveAsync(Villa villa);
        Task SaveAsync();
    }
}
