using Hotel.Domain.Entities;

namespace Hotel.Application.Services.Interfaces
{
    public interface IVillaServices
    {
        IEnumerable<Villa> GetAllVilla(string? includeProp = null);
        Task<Villa> GetVillaById(int id, string? includeProp = null);
        Task CreateAsync(Villa villa);
        Task UpdateAsync(Villa villa);
        Task<bool> Delete(int id);
        IEnumerable<Villa> GetVillasByDate(int nights, DateOnly checkInDate);
        bool IsVillaAvailableByDate(int villaId, int nights, DateOnly checkInDate);
    }
}
