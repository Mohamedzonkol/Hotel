using Hotel.Application.Common.InterFaces;
using Hotel.Application.Services.Interfaces;
using Hotel.Domain.Entities;

namespace Hotel.Application.Services.Implementation
{
    public class VillaNumberServices(IUnitOfWork unit) : IVIllaNumberServices
    {
        public IEnumerable<VillaNumber> GetAllVillaNumber(string? includeProp = null)
        {
            return unit.VillaNumberRepository.GetAllAsync(includeProperty: includeProp);
        }

        public async Task<VillaNumber> GetVillaNumberById(int id/*, string? includeProp1 = null, string? includeProp2 = null*/)
        {
            return await unit.VillaNumberRepository.GetAsync(x => x.Villa_Number == id);
        }

        public async Task CreateVillaNumberAsync(VillaNumber villaNumber)
        {
            await unit.VillaNumberRepository.AddAsync(villaNumber);
        }

        public async Task UpdateAsync(VillaNumber villaNumber)
        {
            await unit.VillaNumberRepository.UpdateAsync(villaNumber);
        }

        public async Task<bool> Delete(int id)
        {
            try
            {
                var villaNumberObj = await unit.VillaNumberRepository.GetAsync(x => x.Villa_Number == id);

                if (villaNumberObj is not null)
                {
                    await unit.VillaNumberRepository.RemoveAsync(villaNumberObj);
                    return true;
                }

                return false;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> CheckVillaNumberExist(int villaNumber)
        {
            return await unit.VillaNumberRepository.AnyAsync(x => x.Villa_Number == villaNumber);

        }
    }
}
