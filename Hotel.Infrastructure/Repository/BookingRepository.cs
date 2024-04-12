using Hotel.Application.Common.InterFaces;
using Hotel.Application.Utility;
using Hotel.Domain.Entities;
using Hotel.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Hotel.Infrastructure.Repository
{
    public class BookingRepository(ApplicationDbContext context) : Repository<Booking>(context), IBookingRepository
    {
        public async Task UpdateAsync(Booking booking)
        {
            context.Bookings.Update(booking);
            await context.SaveChangesAsync();
        }

        public async Task UpdateStatusAsync(int bookingId, string bookingStatus)
        {
            var bookingFromDb = await context.Bookings.FirstOrDefaultAsync(x => x.Id == bookingId);
            if (bookingFromDb is not null)
            {
                bookingFromDb.Status = bookingStatus;
                if (bookingStatus == SD.StatusCheckedIn)
                    bookingFromDb.ActualCheckInDate = DateTime.Now;
                if (bookingStatus == SD.StatusCompleted)
                    bookingFromDb.ActualCheckOutDate = DateTime.Now;
            }

            await context.SaveChangesAsync();
        }

        public async Task UpdateStripePaymentId(int bookingId, string sessionId, string paymentIntentId)
        {
            var bookingFromDb = await context.Bookings.FirstOrDefaultAsync(x => x.Id == bookingId);
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

            await context.SaveChangesAsync();

        }

    }
}
