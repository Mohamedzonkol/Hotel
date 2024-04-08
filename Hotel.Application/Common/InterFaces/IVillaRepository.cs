using Hotel.Domain.Entities;

namespace Hotel.Application.Common.InterFaces
{
    public interface IVillaRepository : IRepository<Villa>
    {
        Task UpdateAsync(Villa villa);
        Task SaveAsync();
    }
}
