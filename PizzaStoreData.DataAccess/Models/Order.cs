using System;
using System.Collections.Generic;
using System.Text;

namespace PizzaStoreData.DataAccess.Models
{
    public class Order
    {
        public Order()
        {
            OrderJunction = new HashSet<OrderJunction>();
        }

        public int      Id         { get; set; }
        public int      LocationId { get; set; }
        public int      UserId     { get; set; }
        public DateTime TimePlaced { get; set; }
        public decimal  TotalPrice { get; set; }

        // Navigation Property
        public ICollection<OrderJunction> OrderJunction { get; set; }
    }
}
