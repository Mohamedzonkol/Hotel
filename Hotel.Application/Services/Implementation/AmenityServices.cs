using Hotel.Application.Common.InterFaces;
using Hotel.Application.Services.Interfaces;
using Hotel.Domain.Entities;

namespace Hotel.Application.Services.Implementation
{
    public class AmenityServices(IUnitOfWork unit) : IAmenityServices
    {
        public IEnumerable<Amenity> GetAllAmenity(string? includeProp = null)
        {
            return unit.AmenityRepository.GetAllAsync(includeProperty: includeProp);
        }

        public async Task<Amenity> GetAmenityById(int id/*, string? includeProp1 = null*/)
        {
            return await unit.AmenityRepository.GetAsync(x => x.Id == id);
        }

        public async Task CreateAsync(Amenity amenity)
        {
            await unit.AmenityRepository.AddAsync(amenity);
        }

        public async Task UpdateAsync(Amenity amenity)
        {
            await unit.AmenityRepository.UpdateAsync(amenity);
        }

        public async Task<bool> Delete(int id)
        {
            try
            {
                var amenityObj = await unit.AmenityRepository.GetAsync(x => x.Id == id);

                if (amenityObj is not null)
                {
                    await unit.AmenityRepository.RemoveAsync(amenityObj);
                    return true;
                }

                return false;
            }
            catch
            {
                return false;
            }
        }
    }
}
