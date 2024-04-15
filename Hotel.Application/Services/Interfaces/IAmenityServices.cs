using Hotel.Domain.Entities;

namespace Hotel.Application.Services.Interfaces
{
    public interface IAmenityServices
    {
        IEnumerable<Amenity> GetAllAmenity(string? includeProp1 = null);
        Task<Amenity> GetAmenityById(int id/*, string? includeProp1 = null*/);
        Task CreateAsync(Amenity amenity);
        Task UpdateAsync(Amenity amenity);
        Task<bool> Delete(int id);
    }
}
