using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ks.Batch.Printer
{
    public class ShoppingService
    {
        public class ShoppingCart
        {
            public int TicketNumber { get; set; }
            public string UserName { get; set; }
            public string Customer { get; set; }
            public List<Details> Details { get; set; }
            public double Total { get; set; }
        }

        public class Details
        {
            public double Quantity { get; set; }
            public string ProductName { get; set; }
            public double Price { get; set; }
            public double SubTotal { get { return Price * Quantity; } }

        }
            }
}
