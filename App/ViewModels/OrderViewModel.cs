using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace App.ViewModels
{
    public class OrderViewModel
    {
        public OrderViewModel()
        {
            Items = new List<OrderItemViewModel>();
        }

        public int Id { get; set; }

        public DateTime OrderDate { get; set; }

        public DateTime Created { get; set; }

        public int MadeBy { get; set; }

        public CompactUser UserMadeBy { get; set; }

        public int ExecutedBy { get; set; }

        public CompactUser UserExecutedBy { get; set; }

        public int TotalQuantity { get; set; }

        public decimal TotalCost { get; set; }

        public bool IsClosed { get; set; }

        public bool IsPaid { get; set; }

        public List<OrderItemViewModel> Items { get; set; }
    } 
}
