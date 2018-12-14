using System;
using Xunit;
using db = PizzaStoreData.DataAccess;

namespace PizzaStoreTesting.Test.RepositoryTesting
{
    public class LocationRepo
    {
        [Fact]
        public void LocationDeleteRemovesFromOrderAndOrderJunctionTables()
        {
            // Arrange
            RepoTesting repo = new RepoTesting();
            repo.ResetDatabase("Location_Test_1");
            int orderId = repo.dbOrder.Id;
            int pizzaId = repo.dbPizza.PizzaId;
            int ingredientId = repo.dbIngredient.Id;
            int locationId = repo.dbLocation.Id;

            // Ensure the entities exist
            Assert.NotNull(repo.orderRepo.GetById(orderId));
            Assert.NotNull(repo.pizzaRepo.GetById(pizzaId, ingredientId));
            Assert.NotNull(repo.orderJunctionRepo.GetById(orderId, pizzaId));

            // Act

            repo.locationRepo.Delete(repo.dbLocation);
            repo.SaveChanges();


            // Assert
            // Order was removed
            Assert.Throws<db.InvalidIdException>(() => repo.orderRepo.GetById(orderId));
            // Order junction was removed              
            Assert.Throws<db.InvalidIdException>(() => repo.orderJunctionRepo.GetById(orderId, pizzaId));
            // Location was removed
            Assert.Throws<db.InvalidIdException>(() => repo.locationRepo.GetById(locationId));

        }
        [Fact]
        public void LocationDeleteRemovesFromUserTable()
        {
            // Arrange
            RepoTesting repo = new RepoTesting();
            repo.ResetDatabase("Location_Test_2");
            int? userDefaultLocationId = repo.dbUser.DefaultLocationId;
            int locationId = repo.dbLocation.Id;

            // Act

            repo.locationRepo.Delete(repo.dbLocation);
            repo.SaveChanges();


            // Assert
            // User was updated
            Assert.NotNull(userDefaultLocationId);
            Assert.Null(repo.dbUser.DefaultLocation);
            // Location was removed
            Assert.Throws<db.InvalidIdException>(() => repo.locationRepo.GetById(locationId));
        }
        [Fact]
        public void LocationDeleteRemovesFromInventoryJunctionTable()
        {
            // Arrange
            RepoTesting repo = new RepoTesting();
            repo.ResetDatabase("Location_Test_3");
            int locationId = repo.dbLocation.Id;
            int ingredientId = repo.dbIngredient.Id;

            // Ensure the entities exist
            Assert.NotNull(repo.locationRepo.GetById(locationId));
            Assert.NotNull(repo.ingredientRepo.GetById(ingredientId));
            Assert.NotNull(repo.inventoryRepo.GetById(locationId, ingredientId));

            // Act
            repo.locationRepo.Delete(repo.dbLocation);
            repo.SaveChanges();


            // Assert
            // InventoryJunction was removed
            Assert.Throws<db.InvalidIdException>(() => repo.inventoryRepo.GetById(locationId, ingredientId));
            // Location was removed
            Assert.Throws<db.InvalidIdException>(() => repo.locationRepo.GetById(locationId));

        }
    }
}
