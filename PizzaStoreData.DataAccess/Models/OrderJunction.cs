using System;
using System.Collections.Generic;
using System.Text;

namespace PizzaStoreData.DataAccess.Models
{
    public class OrderJunction
    {
        public int OrderId { get; set; }
        public int PizzaId { get; set; }
    }
}
