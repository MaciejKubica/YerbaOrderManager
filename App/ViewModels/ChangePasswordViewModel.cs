using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.ViewModels
{
    public class ChangePasswordViewModel
    {
        [Required]
        public string Email { get; set; }

        [Required]        
        public string OldPassword { get; set; }

        [Required]
        public string NewPassword { get; set; }
    }
}
