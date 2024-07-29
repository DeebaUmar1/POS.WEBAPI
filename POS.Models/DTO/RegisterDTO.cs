

using POS.Models.Entities;
using System.ComponentModel.DataAnnotations;

namespace POS.Models.DTO
{
    public class RegisterDTO
    {
        [Required]
        [StringLength(100)] // Assuming a reasonable length for name
        public string name { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(256)] // Maximum length for email
        public string email { get; set; }

        [Required]
        [StringLength(256)] // Assuming a maximum length for password
        public string password { get; set; }

        //[Required]
        public UserRole role { get; set; }
      
    }
}
