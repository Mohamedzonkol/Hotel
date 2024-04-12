using Hotel.Domain.Entities;

namespace Hotel.Application.Common.InterFaces
{
    public interface IBookingRepository : IRepository<Booking>
    {
        Task UpdateAsync(Booking booking);
        Task UpdateStatusAsync(int bookingId, string bookingStatus, int villaNumber);
        Task UpdateStripePaymentId(int bookingId, string sessionId, string paymentIntentId);
    }
}
