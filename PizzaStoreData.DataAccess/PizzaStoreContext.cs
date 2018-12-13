using Microsoft.EntityFrameworkCore;
using PizzaStoreData.DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace PizzaStoreData.DataAccess
{
    public class PizzaStoreContext : DbContext
    {
        // Call base constructor with the correct options 
        //  in order to have this class connect to the 
        //  DB through the connectionstring used to construct
        //  the options provided.
        public PizzaStoreContext(DbContextOptions<PizzaStoreContext> options) 
            : base(options) { }

        #region Standard Tables
        // Table of all possible ingredients a location can
        //  have stocked with their respective price
        public DbSet<Ingredient> Ingredient { get; set; }
        // Table of all location names
        public DbSet<Location>   Location   { get; set; }
        // Table of all orders with what location it was placed
        //  at, what user placed it, time placed, and total
        //  cost of the order
        public DbSet<Order>      Order      { get; set; }
        // Table of all users with first and last names, 
        //  and potential default location to order from
        public DbSet<User>       User       { get; set; }
        #endregion

        #region Junction Tables
        // Can have any number of locations which each have
        //  any number of ingredients
        public DbSet<InventoryJunction> InventoryJunction { get; set; }
        // Can have any number of orders which each have any
        //  number of pizzas
        public DbSet<OrderJunction>     OrderJunction     { get; set; }
        // Can have any number of pizzas which each can have
        //  any number of ingredients
        public DbSet<PizzaJunction>     PizzaJunction     { get; set; }
        #endregion
    }
}
