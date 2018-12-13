using System;
using System.Collections.Generic;
using System.Text;

namespace PizzaStoreData.DataAccess.Models
{
    public class Ingredient
    {
        public int     Id    { get; set; }
        public decimal Price { get; set; }
        public string  Name  { get; set; }
    }
}
