using POS.Models.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS.Models.DTO
{
    public class SetRoleModelDTO
    {
        [Required]
        [StringLength(100)]
        public required string username {  get; set; }

        [Required]
        [RoleValidation]
        public string role { get; set; }

       
        
    }
}
