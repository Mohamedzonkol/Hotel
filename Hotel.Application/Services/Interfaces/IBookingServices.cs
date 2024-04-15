using Hotel.Domain.Entities;

namespace Hotel.Application.Services.Interfaces
{
    public interface IBookingServices
    {
        IEnumerable<Booking> GetBookingList(string? userId = "", string? statusFilterList = "");
        Task<Booking> GetBooking(int id);
        Task Create(Booking booking);
        Task UpdateStatusAsync(int bookingId, string bookingStatus, int villaNumber);
        Task UpdateStripePaymentId(int bookingId, string sessionId, string paymentIntentId);
        IEnumerable<int> GetCheckInVillaNumbers(int id);
    }
}
