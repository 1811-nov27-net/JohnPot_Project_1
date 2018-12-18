using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace PizzaStoreData.DataAccess.Models
{
    public class OrderJunction
    {

        [Key, Column(Order = 0), ]
        public int OrderId { get; set; }
        [Key, Column(Order = 1)]
        public int PizzaId { get; set; }

        //public virtual Order Order { get; set; }
        //public virtual PizzaJunction PizzaJunction { get; set; }
    }
}
