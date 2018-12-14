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
            Location = new Location();
            User = new User();

        }

        public int      Id         { get; set; }
        public int      LocationId { get; set; }
        public int      UserId     { get; set; }
        public DateTime TimePlaced { get; set; }
        public decimal  TotalPrice { get; set; }

        // Navigation Properties
        public virtual Location Location { get; set; }
        public virtual User         User { get; set; }
        public virtual ICollection<OrderJunction> OrderJunction { get; set; }
    }
}
