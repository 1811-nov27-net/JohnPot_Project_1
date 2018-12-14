using System;
using System.Collections.Generic;
using System.Text;

namespace PizzaStoreData.DataAccess.Models
{
    public class Ingredient
    {
        public Ingredient()
        {
            InventoryJunction = new HashSet<InventoryJunction>();
            PizzaJunction     = new HashSet<PizzaJunction>();
        }

        public int     Id    { get; set; }
        public decimal Price { get; set; }
        public string  Name  { get; set; }

        // Navigation properties
        public ICollection<InventoryJunction> InventoryJunction { get; set; }
        public ICollection<PizzaJunction>     PizzaJunction     { get; set; }
    }
}
