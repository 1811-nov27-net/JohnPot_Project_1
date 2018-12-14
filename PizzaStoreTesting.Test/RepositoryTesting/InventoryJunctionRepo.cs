using System;
using Xunit;
using db = PizzaStoreData.DataAccess;
using dbm = PizzaStoreData.DataAccess.Models;

namespace PizzaStoreTesting.Test.RepositoryTesting
{
    public class InventoryJunctionRepo
    {
        [Fact]
        public void CreatingNewInventoryFailsWithInvalidLocation()
        {
            // Arrange
            RepoTesting repo = new RepoTesting();
            repo.ResetDatabase("Inventory_Test_1");
            int ingredientId = repo.dbIngredient.Id;
            int invalidLocationId = -1;

            // Ensure the entities exist
            Assert.NotNull(repo.ingredientRepo.GetById(ingredientId));
            // Searching for this inventory should fail since we 
            //  are passing in an invalid location id
            Assert.Throws<db.InvalidIdException>(() => repo.inventoryRepo.GetById(invalidLocationId, ingredientId));

            db.Models.InventoryJunction dbInventory = new db.Models.InventoryJunction
            { LocationId = invalidLocationId, IngredientId = ingredientId, Count = 1 };


            // Act
            // Assert
            Assert.Throws<db.InvalidIdException>(() => repo.inventoryRepo.Create(dbInventory));
        }
        [Fact]
        public void CreatingNewInventoryFailsWithInvalidIngredient()
        {
            // Arrange
            RepoTesting repo = new RepoTesting();
            repo.ResetDatabase("Inventory_Test_2");
            int locationId = repo.dbLocation.Id;
            int invalidIngredientId = -1;

            // Ensure the entities exist
            Assert.NotNull(repo.locationRepo.GetById(locationId));
            // Searching for this inventory should fail since we 
            //  are passing in an invalid ingredient id
            Assert.Throws<db.InvalidIdException>(() => repo.inventoryRepo.GetById(locationId, invalidIngredientId));

            db.Models.InventoryJunction dbInventory = new db.Models.InventoryJunction
            { LocationId = locationId, IngredientId = invalidIngredientId, Count = 1 };


            // Act
            // Assert
            Assert.Throws<db.InvalidIdException>(() => repo.inventoryRepo.Create(dbInventory));
        }
        [Fact]
        public void CreatingNewInventorySucceedsWithValidLocationAndIngredient()
        {
            // Arrange
            RepoTesting repo = new RepoTesting();
            repo.ResetDatabase("Inventory_Test_3");
            dbm.Location dbLocation = new dbm.Location { Name = "a" };
            dbm.Ingredient dbIngredient = new dbm.Ingredient { Name = "pepperoni", Price = 2.0m };
            repo.locationRepo.Create(dbLocation);
            repo.ingredientRepo.Create(dbIngredient);
            repo.SaveChanges();

            // Ensure the required entities exist
            Assert.NotNull(repo.locationRepo.GetById(dbLocation.Id));
            Assert.NotNull(repo.ingredientRepo.GetById(dbIngredient.Id));

            dbm.InventoryJunction dbInventory = new dbm.InventoryJunction
            { LocationId = dbLocation.Id, IngredientId = dbIngredient.Id, Count = 1};

            // Act
            // Create the new inventory
            repo.inventoryRepo.Create(dbInventory);

            // Assert
            // Searching for this inventory should now succeed
            Assert.NotNull(repo.inventoryRepo.GetById(dbLocation.Id, dbIngredient.Id));
        }
    }
}
