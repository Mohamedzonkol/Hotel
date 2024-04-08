using Hotel.Domain.Entities;

namespace Hotel.Application.Common.InterFaces
{
    public interface IVillaNumberRepository : IRepository<VillaNumber>
    {
        Task UpdateAsync(VillaNumber villa);
        Task SaveAsync();
    }
}
