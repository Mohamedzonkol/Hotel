using Hotel.Application.Common.InterFaces;
using Hotel.Domain.Entities;
using Hotel.Infrastructure.Data;
using Hotel.Infrastructure.Repository;

namespace Hotel.Application.Common.Implementation
{
    internal class VillaNumberRepository(ApplicationDbContext context) : Repository<VillaNumber>(context), IVillaNumberRepository
    {
        public async Task UpdateAsync(VillaNumber villa)
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
