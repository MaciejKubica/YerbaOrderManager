using System;
using System.Collections.Generic;

namespace App.Data.Entities
{
    public class User
    {
        public int Id { get; set; }

        public string Email { get; set; }
        
        public string Name { get; set; }

        public string Password { get; set; }        

        public bool OrderTokenLocker { get; set; }
        
        public string BankAccount { get; set; }
        
        public DateTime? LockoutEndDateUtc { get; set; }

        public bool LockoutEnabled { get; set; }

        public int AccessFailedCount { get; set; }

        public bool IsDeleted { get; set; }

        public IEnumerable<Role> UserRoles { get; set; }

    }
}
