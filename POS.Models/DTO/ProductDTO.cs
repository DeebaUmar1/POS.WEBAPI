using System.ComponentModel.DataAnnotations;

namespace POS.Models.DTO
{
    public class ProductDTO
    {
        public string productID { get; set; }
        [Required]
        [StringLength(100)] // Assuming a maximum length for the name
        public string Name { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than zero.")]
        public double Price { get; set; }

        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Quantity cannot be negative.")]
        public int Quantity { get; set; }

        [Required]
        [StringLength(50)] // Assuming a maximum length for the type
        public string Type { get; set; }

        [Required]
        [StringLength(50)] // Assuming a maximum length for the category
        public string Category { get; set; }
    }

}
