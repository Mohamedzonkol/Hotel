using Hotel.Application.Services.Interfaces;
using Hotel.Application.Utility;
using Hotel.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Stripe.Checkout;
using Syncfusion.DocIO;
using Syncfusion.DocIO.DLS;
using Syncfusion.DocIORenderer;
using Syncfusion.Drawing;
using Syncfusion.Pdf;
using System.Security.Claims;

namespace HotelWeb.Controllers
{
    [Authorize]
    public class BookingController(IBookingServices bookingServices,
        IVillaServices villaServices, IVIllaNumberServices vIllaNumberServices,
        IWebHostEnvironment webHost, UserManager<ApplicationUser> userManager) : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        public async Task<IActionResult> FinalizeBooking(int villaId, DateOnly checkInDate, int nights)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity!;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            ApplicationUser user = await userManager.FindByIdAsync(userId);
            Booking booking = new()
            {
                VillaId = villaId,
                Villa = await villaServices.GetVillaById(villaId, includeProp: "VillaAmenities"),
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
        [HttpPost]
        public async Task<IActionResult> FinalizeBooking(Booking booking)
        {
            var villa = await villaServices.GetVillaById(booking.VillaId);
            booking.TotalCost = villa.Price * booking.Nights;
            booking.Status = SD.StatusPending;
            booking.BookingDate = DateTime.Now;
            if (!villaServices.IsVillaAvailableByDate(villa.Id, booking.Nights, booking.CheckInDate))
            {
                TempData["error"] = "Room Has Been Sold Out .";
                return RedirectToAction("FinalizeBooking", new
                {
                    villaId = booking.VillaId,
                    checkInDate = booking.CheckInDate,
                    nights = booking.Nights
                });

            }
            await bookingServices.Create(booking);
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

                    },
                },
                Quantity = 1,
            });
            var service = new SessionService();
            Session session = await service.CreateAsync(options);
            await bookingServices.UpdateStripePaymentId(booking.Id, session.Id, session.PaymentIntentId);
            Response.Headers.Add("Location", session.Url);
            return new StatusCodeResult(303);
            //return RedirectToAction("BookingConfirmation", new { bookingId = booking.Id });
        }
        public async Task<IActionResult> BookingConfirmation(int bookingId)
        {
            Booking bookingFromDb = await bookingServices.GetBooking(bookingId);
            if (bookingFromDb.Status == SD.StatusPending)
            {
                var service = new SessionService();
                Session session = await service.GetAsync(bookingFromDb.StripeSessionId);
                if (session.PaymentStatus == "paid")
                {
                    await bookingServices.UpdateStatusAsync(bookingFromDb.Id, SD.StatusApproved, 0);
                    await bookingServices.UpdateStripePaymentId(bookingFromDb.Id, session.Id, session.PaymentIntentId);
                }
            }
            return View(bookingId);
        }
        public async Task<IActionResult> BookingDetails(int bookingId)
        {
            Booking bookingFromDb = await bookingServices.GetBooking(bookingId);
            if (bookingFromDb.VillaNumber == 0 && bookingFromDb.Status == SD.StatusApproved)
            {
                var availableVillaNumber = AssignAvailableVillaNumberByVilla(bookingFromDb.VillaId);
                bookingFromDb.VillaNumbers = vIllaNumberServices.GetAllVillaNumber().Where(u => u.VillaId == bookingFromDb.VillaId
                    && availableVillaNumber.Any(x => x == u.Villa_Number)).ToList();

            }
            return View(bookingFromDb);
        }
        [HttpPost]
        [Authorize(Roles = SD.Role_Admin)]
        public async Task<IActionResult> CheckIn(Booking booking)
        {
            await bookingServices.UpdateStatusAsync(booking.Id, SD.StatusCheckedIn, booking.VillaNumber);
            TempData["Success"] = "Booking Updated Successfully .";
            return RedirectToAction("BookingDetails", new { bookingId = booking.Id });
        }
        [HttpPost]
        [Authorize(Roles = SD.Role_Admin)]
        public async Task<IActionResult> CheckOut(Booking booking)
        {
            await bookingServices.UpdateStatusAsync(booking.Id, SD.StatusCompleted, booking.VillaNumber);
            TempData["Success"] = "Booking Completed Successfully .";
            return RedirectToAction("BookingDetails", new { bookingId = booking.Id });
        }
        [HttpPost]
        [Authorize(Roles = SD.Role_Admin)]
        public async Task<IActionResult> CancelBooking(Booking booking)
        {
            await bookingServices.UpdateStatusAsync(booking.Id, SD.StatusCancelled, 0);
            TempData["Success"] = "Booking Cancelled Successfully .";
            return RedirectToAction("BookingDetails", new { bookingId = booking.Id });
        }
        [HttpPost]
        public async Task<IActionResult> GenerateInvoice(int id, string downloadType)
        {
            string basePath = webHost.WebRootPath;//wwwroot
            WordDocument wordDocument = new();
            //Load Template
            string dataPath = basePath + @"/exports/BookingDetails.docx";
            await using FileStream fileStream = new(dataPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            wordDocument.Open(fileStream, FormatType.Automatic);
            //Update  Template
            Booking bookingFromDb = await bookingServices.GetBooking(id);

            TextSelection textSelection = wordDocument.Find("xx_customer_name", false, true);
            WTextRange textRange = textSelection.GetAsOneRange();
            textRange.Text = bookingFromDb.Name;
            textSelection = wordDocument.Find("xx_customer_phone", false, true);
            textRange = textSelection.GetAsOneRange();
            textRange.Text = bookingFromDb.Phone;
            textSelection = wordDocument.Find("xx_customer_email", false, true);
            textRange = textSelection.GetAsOneRange();
            textRange.Text = bookingFromDb.Email;

            textSelection = wordDocument.Find("XX_BOOKING_NUMBER", false, true);
            textRange = textSelection.GetAsOneRange();
            textRange.Text = "BOOKING ID - " + bookingFromDb.Id;
            textSelection = wordDocument.Find("XX_BOOKING_DATE", false, true);
            textRange = textSelection.GetAsOneRange();
            textRange.Text = "BOOKING DATE - " + bookingFromDb.BookingDate.ToShortDateString();

            textSelection = wordDocument.Find("xx_payment_date", false, true);
            textRange = textSelection.GetAsOneRange();
            textRange.Text = bookingFromDb.PaymentDateTime.ToShortDateString();
            textSelection = wordDocument.Find("xx_checkin_date", false, true);
            textRange = textSelection.GetAsOneRange();
            textRange.Text = bookingFromDb.CheckInDate.ToShortDateString();
            textSelection = wordDocument.Find("xx_checkout_date", false, true);
            textRange = textSelection.GetAsOneRange();
            textRange.Text = bookingFromDb.CheckOutDate.ToShortDateString(); ;
            textSelection = wordDocument.Find("xx_booking_total", false, true);
            textRange = textSelection.GetAsOneRange();
            textRange.Text = bookingFromDb.TotalCost.ToString("c");

            WTable table = new(wordDocument);
            table.TableFormat.Borders.LineWidth = 1f;
            table.TableFormat.Borders.Color = Color.Black;
            table.TableFormat.Paddings.Top = 7f;
            table.TableFormat.Paddings.Bottom = 7f;
            table.TableFormat.Borders.Horizontal.LineWidth = 1f;

            int rows = bookingFromDb.VillaNumber > 0 ? 3 : 2;
            table.ResetCells(rows, 4);

            WTableRow row0 = table.Rows[0];

            row0.Cells[0].AddParagraph().AppendText("NIGHTS");
            row0.Cells[0].Width = 80;
            row0.Cells[1].AddParagraph().AppendText("VILLA");
            row0.Cells[1].Width = 220;
            row0.Cells[2].AddParagraph().AppendText("PRICE PER NIGHT");
            row0.Cells[3].AddParagraph().AppendText("TOTAL");
            row0.Cells[3].Width = 80;
            WTableRow row1 = table.Rows[1];

            row1.Cells[0].AddParagraph().AppendText(bookingFromDb.Nights.ToString());
            row1.Cells[0].Width = 80;
            row1.Cells[1].AddParagraph().AppendText(bookingFromDb.Villa.Name);
            row1.Cells[1].Width = 220;
            row1.Cells[2].AddParagraph().AppendText((bookingFromDb.TotalCost / bookingFromDb.Nights).ToString("c"));
            row1.Cells[3].AddParagraph().AppendText(bookingFromDb.TotalCost.ToString("c"));
            row1.Cells[3].Width = 80;
            if (bookingFromDb.VillaNumber > 0)
            {
                WTableRow row2 = table.Rows[2];

                row2.Cells[0].Width = 80;
                row2.Cells[1].AddParagraph().AppendText("Villa Number - " + bookingFromDb.VillaNumber.ToString());
                row2.Cells[1].Width = 220;
                row2.Cells[3].Width = 80;
            }

            WTableStyle tableStyle = wordDocument.AddTableStyle("CustomStyle") as WTableStyle;
            tableStyle.TableProperties.RowStripe = 1;
            tableStyle.TableProperties.ColumnStripe = 2;
            tableStyle.TableProperties.Paddings.Top = 2;
            tableStyle.TableProperties.Paddings.Bottom = 1;
            tableStyle.TableProperties.Paddings.Left = 5.4f;
            tableStyle.TableProperties.Paddings.Right = 5.4f;

            ConditionalFormattingStyle firstRowStyle = tableStyle.ConditionalFormattingStyles.Add(ConditionalFormattingType.FirstRow);
            firstRowStyle.CharacterFormat.Bold = true;
            firstRowStyle.CharacterFormat.TextColor = Color.FromArgb(255, 255, 255, 255);
            firstRowStyle.CellProperties.BackColor = Color.Black;
            table.ApplyStyle("CustomStyle");
            TextBodyPart bodyPart = new(wordDocument);
            bodyPart.BodyItems.Add(table);
            wordDocument.Replace("<ADDTABLEHERE>", bodyPart, false, false);
            using DocIORenderer renderer = new();
            MemoryStream stream = new();
            if (downloadType == "word")
            {
                wordDocument.Save(stream, FormatType.Docx);
                stream.Position = 0;

                return File(stream, "application/docx", "BookingDetails.docx");
            }
            else
            {
                PdfDocument pdfDocument = renderer.ConvertToPDF(wordDocument);
                pdfDocument.Save(stream);
                stream.Position = 0;
                return File(stream, "application/pdf", "BookingDetails.pdf");
            }

        }
        private List<int> AssignAvailableVillaNumberByVilla(int villaId)
        {
            List<int> availableVillaNumber = new();
            var villaNumbers = vIllaNumberServices.GetAllVillaNumber().Where(x => x.VillaId == villaId);
            var checkInVilla = bookingServices.GetCheckInVillaNumbers(villaId);
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
            IEnumerable<Booking> objBookings;
            string userId = "";
            if (string.IsNullOrEmpty(status))
            {
                status = "";
            }

            if (!User.IsInRole(SD.Role_Admin))
            {
                var claimsIdentity = (ClaimsIdentity)User.Identity;
                userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
            }

            objBookings = bookingServices.GetBookingList(userId, status);

            return Json(new { data = objBookings });
        }
        #endregion
    }
}
