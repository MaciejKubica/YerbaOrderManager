using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.ViewModels
{
    public class CreateOrderViewModel
    {
        public CreateOrderViewModel()
        {
            Items = new List<CreateOrderItemViewModel>();
        }

        public int Id { get; set; }

        public DateTime OrderDate { get; set; }

        public DateTime Created { get; set; }

        public int MadeBy { get; set; }

        public int ExecutedBy { get; set; }

        public int TotalQuantity { get; set; }

        public decimal TotalCost { get; set; }

        public bool IsClosed { get; set; }

        public bool IsPaid { get; set; }

        public List<CreateOrderItemViewModel> Items { get; set; }
    }

    public class CreateOrderItemViewModel
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public int YerbaId { get; set; }

        [Range(0, 500)]
        public int Quantity { get; set; }

        [Required]
        public int OrderId { get; set; }


        public bool IsPaid { get; set; }

        public decimal Cost { get; set; }

        [Required]
        public int UserId { get; set; }

    }
}
