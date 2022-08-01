using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace apiVS.DTOs
{
    public class AdminDTO
    {
        [Required]
        [EmailAddress]
        public string email { get; set; }
    }
}
