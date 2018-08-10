using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace App.Data.Entities
{
    public class OrderItem
    {
        public int Id { get; set; }

        public int YerbaId { get; set; }

        public int Quantity { get; set; }

        public int OrderId { get; set; }

        public bool Paid { get; set; }

        public decimal Cost { get; set; }

        public int UserId { get; set; }

        public User UserDetails { get; set; }

        [JsonIgnore]
        public Yerba Yerba { get; set; }

        [JsonIgnore]
        public Order Order { get; set; }
    }
}
