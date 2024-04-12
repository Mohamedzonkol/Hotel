using Hotel.Application.Common.InterFaces;
using Hotel.Application.Utility;
using Hotel.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe.Checkout;
using System.Security.Claims;

namespace HotelWeb.Controllers
{
    public class BookingController(IUnitOfWork unit) : Controller
    {
        [Authorize]
        public async Task<IActionResult> FinalizeBooking(int villaId, DateOnly checkInDate, int nights)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity!;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            ApplicationUser user = await unit.AppUserRepository.GetAsync(x => x.Id == userId);
            Booking booking = new()
            {
                VillaId = villaId,
                Villa = await unit.VillaRepository.GetAsync(x => x.Id == villaId, includeProperty: "VillaAmenities"),
                Nights = nights,
                CheckInDate = checkInDate,
                CheckOutDate = checkInDate.AddDays(nights),
                UserId = userId,
                Phone = user.PhoneNumber,
                Email = user.Email,
                Name = user.Name
            };
            booking.TotalCost = booking.Villa.Price * nights;
            return View(booking);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> FinalizeBooking(Booking booking)
        {
            var villa = await unit.VillaRepository.GetAsync(x => x.Id == booking.VillaId);
            booking.TotalCost = villa.Price * booking.Nights;
            booking.Status = SD.StatusPending;
            booking.BookingDate = DateTime.Now;
            await unit.BookingRepository.AddAsync(booking);
            var domain = Request.Scheme + "://" + Request.Host.Value + "/";
            var options = new SessionCreateOptions
            {
                LineItems = new List<SessionLineItemOptions>(),
                Mode = "payment",
                SuccessUrl = domain + $"booking/BookingConfirmation?bookingId={booking.Id}",
                CancelUrl = domain + $"booking/FinalizeBooking?villaId={booking.VillaId}&checkInDate={booking.CheckInDate}&nights={booking.Nights}",
            };
            options.LineItems.Add(new SessionLineItemOptions()
            {
                PriceData = new SessionLineItemPriceDataOptions()
                {
                    UnitAmount = (long)booking.TotalCost * 100,
                    Currency = "usd",
                    ProductData = new SessionLineItemPriceDataProductDataOptions()
                    {
                        Name = villa.Name,
                        //Images = new List<string>{domain +villa.ImageUrl},
                    },
                },
                Quantity = 1,
            });
            var service = new SessionService();
            Session session = await service.CreateAsync(options);
            await unit.BookingRepository.UpdateStripePaymentId(booking.Id, session.Id, session.PaymentIntentId);

            Response.Headers.Add("Location", session.Url);
            return new StatusCodeResult(303);
            //return RedirectToAction("BookingConfirmation", new { bookingId = booking.Id });
        }

        [Authorize]
        public async Task<IActionResult> BookingConfirmation(int bookingId)
        {
            Booking bookingFromDb = await unit.BookingRepository.GetAsync(x => x.Id == bookingId
            , includeProperty: "User,Villa");
            if (bookingFromDb.Status == SD.StatusPending)
            {
                var service = new SessionService();
                Session session = await service.GetAsync(bookingFromDb.StripeSessionId);
                if (session.PaymentStatus == "paid")
                {
                    await unit.BookingRepository.UpdateStatusAsync(bookingFromDb.Id, SD.StatusApproved);
                    await unit.BookingRepository.UpdateStripePaymentId(bookingFromDb.Id, session.Id, session.PaymentIntentId);
                }
            }
            return View(bookingId);
        }
    }
}
