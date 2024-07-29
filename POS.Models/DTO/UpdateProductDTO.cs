using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS.Models.DTO
{
    public class UpdateProductDTO
    {
         
        public string Name { get; set; }

      
       
        public double Price { get; set; }

       
     public int Quantity { get; set; }

       
      
        public string Type { get; set; }

       
        public string Category { get; set; }
    }
}
