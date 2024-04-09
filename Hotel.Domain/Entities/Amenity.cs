using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hotel.Domain.Entities
{
    public class Amenity
    {
        public int Id { get; set; }
        [Display(Name = "Amenity Name")]
        public required string Name { get; set; }
        public string? Description { get; set; }
        [ForeignKey("Villa"), Display(Name = "Villa")]
        public int VillaId { get; set; }
        [ValidateNever]
        public Villa Villa { get; set; }
    }
}
