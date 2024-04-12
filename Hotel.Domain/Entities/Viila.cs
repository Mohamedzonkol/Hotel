using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hotel.Domain.Entities
{
    public class Villa
    {
        public int Id { get; set; }
        [MaxLength(100)]
        public required string Name { get; set; }
        [MaxLength(1000)]
        public string? Description { get; set; }
        [Display(Name = "Price Per Night"), Range(10, 100000)]
        public double Price { get; set; }
        public int Square { get; set; }
        [Range(1, 100)]
        public int Occupancy { get; set; }
        [NotMapped]
        public IFormFile? Image { get; set; }
        [Display(Name = "Image Url")]
        public string? ImageUrl { get; set; }
        public DateTime? CreateDate { get; set; }
        public DateTime? UpdateDate { get; set; }
        [ValidateNever]
        public IEnumerable<Amenity> VillaAmenities { get; set; }

        [NotMapped] public bool IsAvailable { get; set; } = true;
    }
}


