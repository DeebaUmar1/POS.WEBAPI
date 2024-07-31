using System.ComponentModel.DataAnnotations;

namespace POS.Models.DTO
{
    public class FinalReceiptDTO
    {

        public List<ReceiptDTO> Receipt { get; set; }

        [Required]
        [StringLength(10)] 
        public string TotalAmount { get; set; }

        [Required]
        public DateTime date { get; set; }
    }

}
