using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder.Extensions;

namespace App.Data.Entities
{
    public class Order
    {
        public Order()
        {
            Items = new List<OrderItem>();
        }

        public int Id { get; set; }

        public DateTime OrderDate { get; set; }

        public DateTime Created { get; set; }

        public int MadeBy { get; set; }

        public User UserMadeBy { get; set; }

        public int ExecutedBy { get; set; }
        
        public User UserExecutedBy { get; set; }

        public int TotalQuantity { get; set; }

        public decimal TotalCost { get; set; }

        public bool IsClosed { get; set; }

        public bool IsPaid { get; set; }

        public List<OrderItem> Items { get; set; }
    }
}
