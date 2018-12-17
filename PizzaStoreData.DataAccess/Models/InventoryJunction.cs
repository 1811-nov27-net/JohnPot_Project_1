using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace PizzaStoreData.DataAccess.Models
{
    public class InventoryJunction
    {

        [Key, Column(Order = 0)]
        public int LocationId   { get; set; }
        [Key, Column(Order = 1)]
        public int IngredientId { get; set; }
        // Amount of ingredient based on IngredientId 
        //  this location has in stock.
        public int Count        { get; set; }

        public virtual Location Location     { get; set; }
        public virtual Ingredient Ingredient { get; set; }
    }
}
