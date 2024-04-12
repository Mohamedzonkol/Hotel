using Hotel.Application.Common.InterFaces;
using Hotel.Application.Utility;
using Hotel.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe.Checkout;
using System.Security.Claims;

namespace HotelWeb.Controllers
{
    [Authorize]
    public class BookingController(IUnitOfWork unit) : Controller
    {
        [Authorize]
        public IActionResult Index()
        {
            return View();
        }
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
                    await unit.BookingRepository.UpdateStatusAsync(bookingFromDb.Id, SD.StatusApproved, 0);
                    await unit.BookingRepository.UpdateStripePaymentId(bookingFromDb.Id, session.Id, session.PaymentIntentId);
                }
            }
            return View(bookingId);
        }

        public async Task<IActionResult> BookingDetails(int bookingId)
        {
            Booking bookingFromDb = await unit.BookingRepository.GetAsync(x => x.Id == bookingId
                , includeProperty: "User,Villa");
            if (bookingFromDb.VillaNumber == 0 && bookingFromDb.Status == SD.StatusApproved)
            {
                var availableVillaNumber = AssignAvailableVillaNumberByVilla(bookingFromDb.VillaId);
                bookingFromDb.VillaNumbers = unit.VillaNumberRepository.GetAllAsync(u => u.VillaId == bookingFromDb.VillaId
                    && availableVillaNumber.Any(x => x == u.Villa_Number)).ToList();

            }
            return View(bookingFromDb);
        }

        private List<int> AssignAvailableVillaNumberByVilla(int villaId)
        {
            List<int> availableVillaNumber = new();
            var villaNumbers = unit.VillaNumberRepository.GetAllAsync(x => x.VillaId == villaId);
            var checkInVilla = unit.BookingRepository.GetAllAsync(x => x.VillaId == villaId &&
                                                                       x.Status == SD.StatusCheckedIn).Select(x => x.VillaNumber);
            foreach (var villaNumber in villaNumbers)
            {
                if (!checkInVilla.Contains(villaNumber.Villa_Number))
                    availableVillaNumber.Add(villaNumber.Villa_Number);
            }

            return availableVillaNumber;
        }

        #region ApiCall
        [HttpGet]
        [Authorize]
        public ActionResult GetAll(string status)
        {
            IEnumerable<Booking> bookings;
            if (User.IsInRole(SD.Role_Admin))
                bookings = unit.BookingRepository.GetAllAsync(includeProperty: "User,Villa");
            else
            {
                var claimsIdentity = (ClaimsIdentity)User.Identity!;
                var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                bookings = unit.BookingRepository.GetAllAsync(x => x.UserId == userId, includeProperty: "User,Villa");
            }
            if (!string.IsNullOrEmpty(status))
                bookings = bookings.Where(x => x.Status.ToLower().Equals(status.ToLower()));
            return Json(new { data = bookings });
        }
        #endregion


    }
}
