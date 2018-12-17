using System;
using System.Collections.Generic;
using System.Text;
using db = PizzaStoreData.DataAccess.Models;
using dbr = PizzaStoreData.DataAccess.Repositories;
using lib = PizzaStoreLibrary.library.Models;
using e = PizzaStoreLibrary.library.Exceptions;
using System.Linq;

namespace PizzaStoreData.DataAccess.Models
{
    public static class Mapper
    {
        private static PizzaStoreDBContext Database;
        public static dbr.InventoryJunctionRepository inventoryRepo;
        public static dbr.IngredientRepository ingredientRepo;
        public static dbr.OrderJunctionRepository orderJunctionRepo;
        public static dbr.PizzaJunctionRepository pizzaRepo;

        public static void InitializeMapper(PizzaStoreDBContext database)
        {
            Database = database ?? throw new ArgumentNullException(nameof(database));
            inventoryRepo = new dbr.InventoryJunctionRepository(database);
            ingredientRepo = new dbr.IngredientRepository(database);
            orderJunctionRepo = new dbr.OrderJunctionRepository(database);
            pizzaRepo = new dbr.PizzaJunctionRepository(database);
        }

        #region Ingredient Mapping
        public static db.Ingredient Map(lib.Ingredient ingredient)
        {
            db.Ingredient dbIngredient = new db.Ingredient
            {
                Id = ingredient.Id,
                Name = ingredient.Name,
                Price = ingredient.Price
            };

            return dbIngredient;
        }

        #endregion

        #region Location Mapping
        public static db.Location Map(lib.Location libLocation, out List<db.InventoryJunction> inventoryList)
        {
            db.Location dbLocation = new db.Location
            {
                Id = libLocation.Id,
                Name = libLocation.Name
                
            };

            inventoryList = new List<db.InventoryJunction>();
            foreach(var inventory in libLocation.Inventory)
            {
                if (inventory.Value == 0)
                    continue;

                db.InventoryJunction dbInventory = new db.InventoryJunction
                {
                    IngredientId = inventory.Key,
                    LocationId = libLocation.Id,
                    Count = inventory.Value
                };

                inventoryList.Add(dbInventory);
            }

            return dbLocation;
        }
        public static lib.Location Map(db.Location dbLocation)
        {
            lib.Location libLocation = new lib.Location
            {
                Id = dbLocation.Id,
                Name = dbLocation.Name
            };

            // Populate the inventory
            List<db.InventoryJunction> inventories = inventoryRepo.GetAllInventoryJunctions();
            foreach(var inventory in inventories)
            {
                libLocation.UpdateInventory(ingredientRepo.GetById(inventory.IngredientId).Name, inventory.Count);
            }

            return libLocation;
        }
        #endregion

        #region User Mapping

        /***** Library -> Database *****/
        public static db.User Map(lib.User libUser)
        {
            db.User dbUser = new db.User
            {
                Id = libUser.Id,
                FirstName = libUser.FirstName,
                LastName = libUser.LastName,
                DefaultLocationId = libUser.DefaultLocationId
            };

            return dbUser;
        }
        /***** Database -> Library *****/
        public static lib.User Map(db.User dbUser)
        {
            lib.User libUser = new lib.User
            {
                Id = dbUser.Id,
                FirstName = dbUser.FirstName,
                LastName = dbUser.LastName,
                DefaultLocationId = dbUser.DefaultLocationId
            };

            return libUser;
        }

        #endregion

        #region Order Mapping

        public static db.Order Map(lib.Order libOrder, 
            out List<db.OrderJunction> orderJunctionList, 
            out List<db.PizzaJunction> pizzaJunctionList)
        {
            db.Order dbOrder = new db.Order
            {
                Id = libOrder.Id,
                LocationId = libOrder.LocationId,
                UserId = libOrder.UserId,
                TimePlaced = libOrder.TimePlaced,
                TotalPrice = libOrder.TotalPrice
            };

            orderJunctionList = new List<db.OrderJunction>();
            pizzaJunctionList = new List<db.PizzaJunction>();
            foreach (var pizza in libOrder.Pizzas)
            {
                db.OrderJunction orderJunction = new db.OrderJunction
                {
                    OrderId = libOrder.Id,
                    PizzaId = pizza.Id
                };
                orderJunctionList.Add(orderJunction);
                foreach(var ingredient in pizza.Ingredients.Distinct())
                {
                    db.PizzaJunction pizzaJunction = new db.PizzaJunction
                    {
                        PizzaId = pizza.Id,
                        IngredientId = ingredient.Id,
                        Count = pizza.Ingredients.Where(i => i.Id == ingredient.Id).Count()
                    };
                    pizzaJunctionList.Add(pizzaJunction);
                }
                
            }
            return dbOrder;
        }

        public static lib.Order Map(db.Order dbOrder)
        {
            lib.Order libOrder = new lib.Order
            {
                Id = dbOrder.Id,
                LocationId = dbOrder.LocationId,
                UserId = dbOrder.UserId,
                TimePlaced = dbOrder.TimePlaced
            };
            List<db.OrderJunction> orderJunctions = orderJunctionRepo.GetAllOrderJunctions();
            foreach (var orderJunction in orderJunctions)
            {
                List<db.PizzaJunction> pizzaJunctions = pizzaRepo.GetAllPizzaJunctions();
                lib.Pizza libPizza = new lib.Pizza
                {
                    Id = orderJunction.PizzaId
                };
                foreach(var pizzaJunction in pizzaJunctions)
                {
                    for (int i = 0; i < pizzaJunction.Count; ++i)
                        libPizza.AddIngredientsToPizza(lib.Ingredient.GetById(pizzaJunction.IngredientId).Name);
                }
                libOrder.AddPizzaToOrder(libPizza);
            }
            return libOrder;
        }

        #endregion  
    }
}
