using Hotel.Domain.Entities;

namespace Hotel.Application.Services.Interfaces
{
    public interface IVIllaNumberServices
    {
        IEnumerable<VillaNumber> GetAllVillaNumber(string? includeProp = null);
        Task<VillaNumber> GetVillaNumberById(int id/*, string? includeProp1 = null, string? includeProp2 = null*/);
        Task CreateVillaNumberAsync(VillaNumber villaNumber);
        Task UpdateAsync(VillaNumber villaNumber);
        Task<bool> Delete(int id);
        Task<bool> CheckVillaNumberExist(int villaNumber);
    }
}
