using System;
using System.Collections.Generic;
using System.Text;

namespace PizzaStoreData.DataAccess.Models
{
    public class Location
    {
        public Location()
        {
            Order             = new HashSet<Order>();
            User              = new HashSet<User>();
            InventoryJunction = new HashSet<InventoryJunction>();
        }

        public int    Id   { get; set; }
        public string Name { get; set; }

        // Navigation Properties
        public ICollection<Order>             Order             { get; set; }
        public ICollection<User>              User              { get; set; }
        public ICollection<InventoryJunction> InventoryJunction { get; set; }
    }
}
