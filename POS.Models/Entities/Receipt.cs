using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS.Models.Entities
{
    public class Receipt
    {
        [Required]
        [StringLength(10)] // Assuming a reasonable length for Quantity
        public string Quantity { get; set; }

        [Required]
        [StringLength(100)] // Assuming a reasonable length for Product
        public string Product { get; set; }

        [Required]
        [StringLength(10)] // Assuming a reasonable length for Price
        public string Price { get; set; }

        [Required]
        [StringLength(10)] // Assuming a reasonable length for Total
        public string Total { get; set; }
    }

    public class FinalReceipt
    {
        public List<Receipt> Receipt { get; set; } = new List<Receipt>();

        [Required]
        [StringLength(10)] // Assuming a reasonable length for TotalAmount
        public string TotalAmount { get; set; }

        [Required]
        public DateTime date { get; set; }
    }


}
