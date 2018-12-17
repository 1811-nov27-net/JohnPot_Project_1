using Microsoft.EntityFrameworkCore;
using PizzaStoreData.DataAccess.Repositories;
using System;
using System.Collections.Generic;
using System.Text;
using db = PizzaStoreData.DataAccess;
using dbm = PizzaStoreData.DataAccess.Models;


namespace PizzaStoreTesting.Test
{
    public class RepoTesting
    {
        public db.PizzaStoreDBContext database;
        public IngredientRepository ingredientRepo;
        public LocationRepository locationRepo;
        public OrderRepository orderRepo;
        public UserRepository userRepo;
        public InventoryJunctionRepository inventoryRepo;
        public PizzaJunctionRepository pizzaRepo;
        public OrderJunctionRepository orderJunctionRepo;

        public dbm.Ingredient dbIngredient;
        public dbm.Location dbLocation;
        public dbm.User dbUser;
        public dbm.InventoryJunction dbInventory;
        public dbm.PizzaJunction dbPizza;
        public dbm.Order dbOrder;
        public dbm.OrderJunction dbOrderJunction;

        public void SaveChanges()
        {
            database.SaveChanges();
        }
        public void ResetDatabase(string dbName)
        {
            var options = new DbContextOptionsBuilder<db.PizzaStoreDBContext>()
                .UseInMemoryDatabase(dbName).Options;
            database = new db.PizzaStoreDBContext(options);
            ingredientRepo = new IngredientRepository(database);
            locationRepo = new LocationRepository(database);
            userRepo = new UserRepository(database);
            inventoryRepo = new InventoryJunctionRepository(database);
            pizzaRepo = new PizzaJunctionRepository(database);
            orderJunctionRepo = new OrderJunctionRepository(database);
            orderRepo = new OrderRepository(database);

            // Going to add one entry to each repo for testing purposes.
            dbIngredient = new dbm.Ingredient { Id = 9999, Name = "cheese", Price = 1.50m };
            ingredientRepo.Create(dbIngredient);
            dbLocation = new dbm.Location { Id = 9999, Name = "John's Pizzaria" };
            locationRepo.Create(dbLocation);
            
            // Save should populate the above entities' Ids
            SaveChanges();

            dbUser = new dbm.User { Id = 9999, FirstName = "John", LastName = "Pot", DefaultLocationId = dbLocation.Id };
            userRepo.Create(dbUser);
            dbInventory = new dbm.InventoryJunction { LocationId = dbLocation.Id, IngredientId = dbIngredient.Id, Count = 100 };
            inventoryRepo.Create(dbInventory);
            // Have to manually set the pizza junction id since it is a nested many-to-many relationship
            Random rand = new Random(DateTime.Now.TimeOfDay.Milliseconds);
            dbPizza = new dbm.PizzaJunction { PizzaId = dbm.PizzaJunction.GetNewId(),  IngredientId = dbIngredient.Id, Count = 2 };
            pizzaRepo.Create(dbPizza);
            
            // Update user id for order usage
            SaveChanges();

            dbOrder = new dbm.Order { Id = 9999, LocationId = dbLocation.Id, UserId = dbUser.Id, TimePlaced = DateTime.Now, TotalPrice = 20.50m };
            orderRepo.Create(dbOrder);
            
            // Order junction needs order to have an Id
            SaveChanges();

            dbOrderJunction = new dbm.OrderJunction { OrderId = dbOrder.Id, PizzaId = dbPizza.PizzaId };
            orderJunctionRepo.Create(dbOrderJunction);

            SaveChanges();
            // All tables should now have one entry.
        }
    }
}
