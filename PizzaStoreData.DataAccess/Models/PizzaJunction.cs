using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace PizzaStoreData.DataAccess.Models
{
    public class PizzaJunction
    {
        [Key, Column(Order = 0)]
        public int PizzaId { get; set; }
        [Key, Column(Order = 1)]
        public int IngredientId { get; set; }
        // Amount of ingredient based on IngredientId
        //  the current pizza with have
        public int Count { get; set; }
    }
}
