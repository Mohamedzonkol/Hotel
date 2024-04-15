using Hotel.Application.Common.InterFaces;
using Hotel.Application.Services.Interfaces;
using Hotel.Application.Utility;
using Hotel.Domain.Entities;

namespace Hotel.Application.Services.Implementation
{
    public class BookingServices(IUnitOfWork unit) : IBookingServices

    {
        public IEnumerable<Booking> GetBookingList(string? userId = "", string? statusFilterList = "")
        {
            IEnumerable<string>? statusList = statusFilterList.ToLower().Split(",");
            if (!string.IsNullOrEmpty(statusFilterList) && !string.IsNullOrEmpty(userId))
                return unit.BookingRepository.GetAllAsync(x => statusList.Contains(x.Status.ToLower()) &&
                                                               x.UserId == userId, includeProperty: "User,Villa");
            else
            {
                if (!string.IsNullOrEmpty(statusFilterList))
                    return unit.BookingRepository.GetAllAsync(x => statusList.Contains(x.Status.ToLower())
                        , includeProperty: "User,Villa");
                if (!string.IsNullOrEmpty(userId))
                    return unit.BookingRepository.GetAllAsync(x => x.UserId == userId
                        , includeProperty: "User,Villa");
            }
            return unit.BookingRepository.GetAllAsync(includeProperty: "User,Villa");
        }
        public async Task<Booking> GetBooking(int id)
        {
            return await unit.BookingRepository.GetAsync(x => x.Id == id, includeProperty: "User,Villa");
        }
        public async Task Create(Booking booking)
        {
            await unit.BookingRepository.AddAsync(booking);
        }
        public async Task UpdateStatusAsync(int bookingId, string bookingStatus, int villaNumber = 0)
        {
            var bookingFromDb = await unit.BookingRepository.GetAsync(x => x.Id == bookingId, tracked: true);
            if (bookingFromDb is not null)
            {
                bookingFromDb.Status = bookingStatus;
                if (bookingStatus == SD.StatusCheckedIn)
                {
                    bookingFromDb.ActualCheckInDate = DateTime.Now;
                    bookingFromDb.VillaNumber = villaNumber;
                }
                if (bookingStatus == SD.StatusCompleted)
                    bookingFromDb.ActualCheckOutDate = DateTime.Now;
            }
            await unit.SaveAsync();
        }
        public async Task UpdateStripePaymentId(int bookingId, string sessionId, string paymentIntentId)
        {
            var bookingFromDb = await unit.BookingRepository.GetAsync(x => x.Id == bookingId, tracked: true);
            if (bookingFromDb is not null)
            {
                if (!string.IsNullOrEmpty(sessionId))
                    bookingFromDb.StripeSessionId = sessionId;
                if (!string.IsNullOrEmpty(paymentIntentId))
                {
                    bookingFromDb.StripePaymentIntentId = paymentIntentId;
                    bookingFromDb.PaymentDateTime = DateTime.Now;
                    bookingFromDb.IsPaymentSuccessfully = true;
                }
            }
            await unit.SaveAsync();
        }
        public IEnumerable<int> GetCheckInVillaNumbers(int id)
        {
            return unit.BookingRepository.GetAllAsync(x => x.VillaId == id &&
                                                                         x.Status == SD.StatusCheckedIn).Select(x => x.VillaNumber);
        }
    }
}
