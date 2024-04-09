using Hotel.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hotel.Application.Common.InterFaces
{
    public interface IAmenityRepository : IRepository<Amenity>
    {
        Task UpdateAsync(Amenity amenity);
    }
}
