using Hotel.Application.Common.InterFaces;
using Hotel.Domain.Entities;
using Hotel.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Hotel.Infrastructure.Repository
{
    public class VillaRepository(ApplicationDbContext context) : IVillaRepository
    {
        public async Task<IEnumerable<Villa>> GetAllAsync(Expression<Func<Villa, bool>>? filter = null, string? includeProperty = null)
        {
            IQueryable<Villa> query = context.Set<Villa>();
            if (filter is not null)
                query = query.Where(filter);
            if (!string.IsNullOrEmpty(includeProperty))
            {
                //Be Careful The include property case sensitive
                foreach (var item in includeProperty.Split(new Char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(item);
                }
            }
            return await query.ToListAsync();
        }

        public async Task<IQueryable<Villa>> GetAsync(Expression<Func<Villa, bool>> filter,
            string? includeProperty = null)
        {
            IQueryable<Villa> query = context.Set<Villa>();
            if (filter is not null)
                query = query.Where(filter);
            if (!string.IsNullOrEmpty(includeProperty))
            {
                foreach (var item in includeProperty.Split(new Char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(item);
                }
            }
            return query;
        }

        public async Task AddAsync(Villa villa)
        {
            await context.AddAsync(villa);
        }

        public async Task UpdateAsync(Villa villa)
        {
            context.Update(villa);
        }

        public async Task RemoveAsync(Villa villa)
        {
            context.Remove(villa);
        }

        public async Task SaveAsync()
        {
            await context.SaveChangesAsync();
        }
    }
}
