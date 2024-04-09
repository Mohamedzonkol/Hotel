using Hotel.Application.Common.InterFaces;
using Hotel.Domain.Entities;
using Hotel.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hotel.Infrastructure.Repository
{
    public class AmenityRepository(ApplicationDbContext context) : Repository<Amenity>(context), IAmenityRepository
    {
        public async Task UpdateAsync(Amenity amenity)
        {
            context.Amenities.Update(amenity);
            await context.SaveChangesAsync();
        }
    }
}
