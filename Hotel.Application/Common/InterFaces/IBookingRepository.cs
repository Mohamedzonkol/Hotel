using Hotel.Domain.Entities;

namespace Hotel.Application.Common.InterFaces
{
    public interface IBookingRepository : IRepository<Booking>
    {
        Task UpdateAsync(Booking booking);

    }
}
