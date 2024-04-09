using Hotel.Domain.Entities;

namespace HotelWeb.ViewModel
{
    public class HomeViewModel
    {
        public IEnumerable<Villa>? VillaList { get; set; }
        public DateOnly CheckInData { get; set; }
        public DateOnly? CheckOutData { get; set; }
        public int Nights { get; set; }
    }
}
