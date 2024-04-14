using Hotel.Application.Common.DTO;
using Hotel.Application.Common.InterFaces;
using Hotel.Application.Services.Interfaces;
using Hotel.Application.Utility;

namespace Hotel.Application.Services.Implementation
{
    public class DashboardServices(IUnitOfWork unit) : IDashboardServices
    {
        static int previousMonth = DateTime.Now.Month == 1 ? 12 : DateTime.Now.Month - 1;
        readonly DateTime previousMonthStartDate = new(DateTime.Now.Year, previousMonth, 1);
        readonly DateTime currentMonthStartDate = new(DateTime.Now.Year, DateTime.Now.Month, 1);
        public async Task<RedialBarChartDto> GetTotalBookingRedialChartData()
        {
            var booking =
                unit.BookingRepository.GetAllAsync(x => x.Status != SD.StatusPending || x.Status == SD.StatusCancelled);
            var countByCurrentMonth = booking.Count(x => x.BookingDate >= currentMonthStartDate && x.BookingDate <= DateTime.Now);
            var countByPreviousMonth = booking.Count(x => x.BookingDate >= previousMonthStartDate && x.BookingDate <= currentMonthStartDate);
            return GetRadialChartDataModel(booking.Count(), countByCurrentMonth, countByPreviousMonth);
        }
        public async Task<RedialBarChartDto> GetRegisteredUserChartData()
        {
            var users =
                unit.AppUserRepository.GetAllAsync();
            var countByCurrentMonth = users.Count(x => x.CreatedAt >= currentMonthStartDate && x.CreatedAt <= DateTime.Now);
            var countByPreviousMonth = users.Count(x => x.CreatedAt >= previousMonthStartDate && x.CreatedAt <= currentMonthStartDate);
            return GetRadialChartDataModel(users.Count(), countByCurrentMonth, countByPreviousMonth);
        }
        public async Task<RedialBarChartDto> GetRevenueChartData()
        {
            var booking =
                unit.BookingRepository.GetAllAsync(x => x.Status != SD.StatusPending || x.Status == SD.StatusCancelled);
            var totalRevenue = Convert.ToInt32(booking.Sum(x => x.TotalCost));
            var countByCurrentMonth = booking.Where(x => x.BookingDate >= currentMonthStartDate && x.BookingDate <= DateTime.Now).Sum(x => x.TotalCost);
            var countByPreviousMonth = booking.Where(x => x.BookingDate >= previousMonthStartDate && x.BookingDate <= currentMonthStartDate).Sum(x => x.TotalCost);
            return GetRadialChartDataModel(totalRevenue, countByCurrentMonth, countByPreviousMonth);
        }
        public async Task<PieChartDTO> GetBookingPieChartData()
        {
            var booking =
                unit.BookingRepository.GetAllAsync(x => x.BookingDate >= DateTime.Now.AddDays(-30) &&
                                                        (x.Status != SD.StatusPending || x.Status == SD.StatusCancelled));
            var customerWithOneBooking = booking.GroupBy(b => b.UserId).Where(x => x.Count() == 1).Select(x => x.Key).ToList();
            int bookingByNewCustomer = customerWithOneBooking.Count();
            int bookingByReturningCustomer = booking.Count() - bookingByNewCustomer;
            PieChartDTO pieChartDto = new()
            {
                Labels = new string[] { " New Customer Bookings ", " Returning Customer Bookings " },
                Series = new Decimal[] { bookingByNewCustomer, bookingByReturningCustomer }
            };
            return pieChartDto;
        }
        public async Task<LineChartDTO> GetMemberAndBookingLineChartData()
        {
            var bookingData =
                unit.BookingRepository.GetAllAsync(
                x => x.BookingDate >= DateTime.Now.AddDays(-30) && x.BookingDate.Date <= DateTime.Now)
                .GroupBy(b => b.BookingDate.Date)
                .Select(u => new
                {
                    DateTime = u.Key,
                    NewBookingCount = u.Count()
                });
            var customerData =
                unit.AppUserRepository.GetAllAsync(
                x => x.CreatedAt >= DateTime.Now.AddDays(-30) && x.CreatedAt.Date <= DateTime.Now)
                .GroupBy(b => b.CreatedAt.Date)
                .Select(u => new
                {
                    DateTime = u.Key,
                    NewCustomerCount = u.Count()
                });
            var leftJoin = bookingData.GroupJoin(customerData, booking => booking.DateTime,
                customer => customer.DateTime, (booking, customer) => new
                {
                    booking.DateTime,
                    booking.NewBookingCount,
                    NewCustomerCount = customer.Select(x => x.NewCustomerCount).FirstOrDefault()
                });
            var rightJoin = customerData.GroupJoin(bookingData, customer => customer.DateTime, booking => booking.DateTime,
                (customer, booking) => new
                {
                    customer.DateTime,
                    customer.NewCustomerCount,
                    NewBookingCount = booking.Select(x => x.NewBookingCount).FirstOrDefault()
                });
            var mergedData = leftJoin
                .Select(x => new { x.DateTime, x.NewBookingCount, x.NewCustomerCount })
                .Union(rightJoin.Select(y => new { y.DateTime, y.NewBookingCount, y.NewCustomerCount }))
                .OrderBy(z => z.DateTime)
                .ToList();
            var newBookingData = mergedData.Select(x => x.NewBookingCount).ToArray();
            var newCustomerData = mergedData.Select(x => x.NewCustomerCount).ToArray();
            var categories = mergedData.Select(x => x.DateTime.ToString("dd-MM-yyyy")).ToArray();
            LineChartDTO lineChartDto = new()
            {
                Categorie = categories,
                Series = new List<ChartData>()
                {
                    new ChartData()
                    {
                        Name = " New Booking ",
                        Data = newBookingData
                    },
                    new ChartData()
                    {
                        Name = " New Members ",
                        Data = newCustomerData
                    }
                }
            };
            return lineChartDto;
        }
        private static RedialBarChartDto GetRadialChartDataModel(int totalCount, double currentMonthCount, double previousMonthCount)
        {
            RedialBarChartDto redialDto = new();
            int increaseDecreaseRatio = 100;
            if (previousMonthCount != 0)
                increaseDecreaseRatio = Convert.ToInt32((currentMonthCount - previousMonthCount) / previousMonthCount * 100);
            redialDto.TotalCount = totalCount;
            redialDto.CountInCurrentMonth = Convert.ToInt32(currentMonthCount);
            redialDto.HasRatioIncreased = currentMonthCount > previousMonthCount;
            redialDto.Series = new int[] { increaseDecreaseRatio };
            return redialDto;
        }

    }
}
