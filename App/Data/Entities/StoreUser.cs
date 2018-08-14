using AspNetCore.Identity.SQLite.Dapper;
using Microsoft.AspNetCore.Identity;

namespace App.Data.Entities
{
    public class StoreUserExtended : IdentityUserCore<int>
    {        
        public bool OrderTokenLocker { get; set; }

        public string BankAccount { get; set; }
    }
}
