using POS.Models.Entities;
using System.ComponentModel.DataAnnotations;

namespace POS.Models.DTO
{
    public class LoginDTO
    {
        [Required]
        [StringLength(100)]
        public required string name { get; set; }
        [Required]
        [StringLength(256)]
        public required string password { get; set; }
      
        public UserRole role { get; set; }
    }
}
