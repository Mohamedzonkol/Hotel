using Hotel.Domain.Entities;

namespace Hotel.Application.Common.InterFaces
{
    public interface IAmenityRepository : IRepository<Amenity>
    {
        Task UpdateAsync(Amenity amenity);
    }
}
