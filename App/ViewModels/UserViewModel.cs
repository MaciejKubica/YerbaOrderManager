using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace App.ViewModels
{
    public class UserViewModel
    {
        [Required]
        public int Id { get; set; }

        [Required]
        [EmailAddress]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]        
        [DataType(DataType.Password)]
        public string Password { get; set; }        

        public bool OrderTokenLocker { get; set; }
        
        public string BankAccount { get; set; }

        public DateTime? LockoutEndDateUtc { get; set; }

        public bool LockoutEnabled { get; set; }

        public int AccessFailedCount { get; set; }

        public bool IsDeleted { get; set; }

        public IEnumerable<RolesViewModel> Roles { get; set; }

        public override string ToString()
        {
            return $"Email: {this.Email}, Name: {this.Name}";
        }
    }
}
