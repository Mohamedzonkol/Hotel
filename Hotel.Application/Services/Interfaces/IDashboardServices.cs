using Hotel.Application.Common.DTO;

namespace Hotel.Application.Services.Interfaces
{
    public interface IDashboardServices
    {
        Task<RedialBarChartDto> GetTotalBookingRedialChartData();
        Task<RedialBarChartDto> GetRegisteredUserChartData();
        Task<RedialBarChartDto> GetRevenueChartData();
        Task<PieChartDTO> GetBookingPieChartData();
        Task<LineChartDTO> GetMemberAndBookingLineChartData();
    }
}
