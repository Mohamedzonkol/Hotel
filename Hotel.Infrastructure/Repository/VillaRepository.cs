using Hotel.Application.Common.InterFaces;
using Hotel.Domain.Entities;
using Hotel.Infrastructure.Data;

namespace Hotel.Infrastructure.Repository
{
    public class VillaRepository(ApplicationDbContext context) : Repository<Villa>(context), IVillaRepository
    {
        public async Task UpdateAsync(Villa villa)
        {
            context.Update(villa);
            await context.SaveChangesAsync();
        }

        public async Task SaveAsync()
        {
            await context.SaveChangesAsync();
        }
    }
}
