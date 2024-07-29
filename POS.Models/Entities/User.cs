using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using static System.Reflection.Metadata.BlobBuilder;
using System.ComponentModel.DataAnnotations.Schema;
using POS.Models.Entities;
using Microsoft.AspNetCore.Identity;

namespace POS.Models.Entities
{
    public class User : IdentityUser
    {
        [Key]
        public int Id { get; set; }

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