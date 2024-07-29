using POS.Models.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS.Models.DTO
{
    public class UserRoleDTO
    {
        [Required]
        public UserRole role { get; set; }
    }
}
