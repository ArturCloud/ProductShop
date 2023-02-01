using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectX_Models.ViewModels
{
    public class OrderViewModel
    {
        public OrderHeader OrderHeaderVM { get; set; }
        public IEnumerable<OrderDetail> OrderDetailsVM { get; set; }
    }
}
