using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using static System.Reflection.Metadata.BlobBuilder;

namespace POS.Models.Entities
{
   
    public class Product
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)] // Assuming a maximum length for the name
        public string name { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than zero.")]
        public double price { get; set; }

        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Quantity cannot be negative.")]
        public int quantity { get; set; }

        [Required]
        [StringLength(50)] // Assuming a maximum length for the type
        public string type { get; set; }

        [Required]
        [StringLength(50)] // Assuming a maximum length for the category
        public string category { get; set; }
    }



}
