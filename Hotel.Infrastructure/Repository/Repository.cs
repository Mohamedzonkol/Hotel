using Hotel.Application.Common.InterFaces;
using Hotel.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Hotel.Infrastructure.Repository
{
    public class Repository<T> : IRepository<T>
        where T : class
    {
        private readonly ApplicationDbContext _context;
        internal DbSet<T> dbSet;
        public Repository(ApplicationDbContext context)
        {
            _context = context;
            dbSet = _context.Set<T>();
        }

        public IEnumerable<T> GetAllAsync(Expression<Func<T, bool>>? filter = null, string? includeProperty = null, bool tracked = false)
        {
            IQueryable<T> query;
            if (tracked)
                query = dbSet;
            else
                query = dbSet.AsNoTracking();


            if (filter is not null)
                query = query.Where(filter);
            if (!string.IsNullOrEmpty(includeProperty))
            {
                //Be Careful The include property case sensitive
                foreach (var item in includeProperty.Split(new Char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(item.Trim());
                }
            }
            return query.ToList();
        }

        public async Task<T> GetAsync(Expression<Func<T, bool>> filter, string? includeProperty = null, bool tracked = false)
        {
            IQueryable<T> query;
            if (tracked)
                query = dbSet;
            else
                query = dbSet.AsNoTracking();
            if (filter is not null)
                query = query.Where(filter);
            if (!string.IsNullOrEmpty(includeProperty))
            {
                foreach (var item in includeProperty.Split(new Char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(item.Trim());
                }
            }
            return await query.FirstOrDefaultAsync();
        }

        public async Task AddAsync(T entity)
        {
            await dbSet.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> AnyAsync(Expression<Func<T, bool>> filter)
        {
            return await dbSet.AnyAsync(filter);
        }

        public async Task RemoveAsync(T entity)
        {
            dbSet.Remove(entity);
            await _context.SaveChangesAsync();
        }
    }
}
