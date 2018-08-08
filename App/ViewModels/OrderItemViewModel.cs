using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.ViewModels
{
    public class OrderItemViewModel
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

        public CompactUser UserDetails { get; set; }

        [JsonIgnore]
        public UserViewModel User { get; set; }
    }
}
