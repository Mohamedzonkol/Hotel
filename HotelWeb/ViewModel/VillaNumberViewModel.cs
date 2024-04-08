using Hotel.Domain.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HotelWeb.ViewModel
{
    public class VillaNumberViewModel
    {
        public VillaNumber? VillaNumber { get; set; }
        public IEnumerable<SelectListItem>? VillaList { get; set; }


    }
}
