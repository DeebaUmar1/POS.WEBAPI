using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace POS.Models.Entities
{
    public class User : IdentityUser
    {
        [Key]  
        [Required]
        public string id {  get; set; }
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

        [Required]
        public UserRole role { get; set; }
    }

}