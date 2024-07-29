using System.ComponentModel.DataAnnotations;

namespace POS.Models.DTO
{
    public class SaleProductsDTO
    {
     
        [Required]
        public DateTime Date { get; set; }

        [Required]
        public int ProductId { get; set; }

        [Required]
        [StringLength(100)] // Assuming a reasonable length for ProductName
        public string ProductName { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be greater than zero.")]
        public int Quantity { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "ProductPrice must be greater than zero.")]
        public double ProductPrice { get; set; }
    }
}
