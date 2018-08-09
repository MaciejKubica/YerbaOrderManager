using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.ViewModels
{
    public class ConfirmPaimentRequest
    {
        public int OrderItemId { get; set; }

        public int UserId { get; set; }
    }
}
