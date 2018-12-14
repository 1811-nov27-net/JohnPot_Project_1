using System;
using Xunit;
using db = PizzaStoreData.DataAccess;
using dbm = PizzaStoreData.DataAccess.Models;

namespace PizzaStoreTesting.Test.RepositoryTesting
{
    public class OrderRepo
    {
        [Fact]
        public void OrderDeleteRemovesFromOrderJunction()
        {
            // Arrange
            RepoTesting repo = new RepoTesting();
            repo.ResetDatabase("Order_Test_1");
            int orderId = repo.dbOrder.Id;
            int pizzaId = repo.dbPizza.PizzaId;
            int ingredientId = repo.dbIngredient.Id;

            // Ensure the entities exist
            Assert.NotNull(repo.orderRepo.GetById(orderId));
            Assert.NotNull(repo.pizzaRepo.GetById(pizzaId, ingredientId));
            Assert.NotNull(repo.orderJunctionRepo.GetById(orderId, pizzaId));

            // Act

            repo.orderRepo.Delete(repo.dbOrder);
            repo.SaveChanges();


            // Assert
            // Order junction was removed              
            Assert.Throws<db.InvalidIdException>(() => repo.orderJunctionRepo.GetById(orderId, pizzaId));
            // Order was removed
            Assert.Throws<db.InvalidIdException>(() => repo.orderRepo.GetById(orderId));
        }
        [Fact]
        public void OrderCreateSucceedsWithValidLocationIdAndUserId()
        {
            // Arrange
            RepoTesting repo = new RepoTesting();
            repo.ResetDatabase("Order_Test_2");
            dbm.Location dbLocation = new dbm.Location { Name = "a" };
            repo.locationRepo.Create(dbLocation);
            repo.SaveChanges();
            dbm.User dbUser = new dbm.User { FirstName = "John", LastName = "Pot", DefaultLocationId = dbLocation.Id };
            repo.userRepo.Create(dbUser);
            repo.SaveChanges();
            // Ensure the entities exist
            Assert.NotNull(repo.locationRepo.GetById(dbLocation.Id));
            Assert.NotNull(repo.userRepo.GetById(dbUser.Id));

            dbm.Order dbOrder = new dbm.Order
            {
                LocationId = dbLocation.Id,
                UserId = dbUser.Id,
                TimePlaced = DateTime.Now,
                TotalPrice = 20.50m
            };

            // Act

            repo.orderRepo.Create(dbOrder);
            repo.SaveChanges();


            // Assert
            // New order should be findable
            Assert.NotNull(repo.orderRepo.GetById(dbOrder.Id));
        }
        [Fact]
        public void OrderCreateFailsWithInvalidLocationId()
        {
            // Arrange
            RepoTesting repo = new RepoTesting();
            repo.ResetDatabase("Order_Test_2");
            dbm.User dbUser = new dbm.User { FirstName = "John", LastName = "Pot"};
            repo.userRepo.Create(dbUser);
            repo.SaveChanges();
            int invalidLocationId = -1;

            // Ensure the user is valid
            Assert.NotNull(repo.userRepo.GetById(dbUser.Id));
            // Ensure the invalidLocationId is in fact invalid
            Assert.Throws<db.InvalidIdException>(() => repo.locationRepo.GetById(invalidLocationId));

            dbm.Order dbOrder = new dbm.Order
            {
                LocationId = invalidLocationId,
                UserId = dbUser.Id,
                TimePlaced = DateTime.Now,
                TotalPrice = 20.50m
            };

            // Act

            Assert.Throws<db.InvalidIdException>(() => repo.orderRepo.Create(dbOrder));


            // Assert
            // New order should not have been added to the database
            Assert.Throws<db.InvalidIdException>(() => repo.orderRepo.GetById(dbOrder.Id));
        }
        [Fact]
        public void OrderCreateFailsWithInvalidUserId()
        {
            // Arrange
            RepoTesting repo = new RepoTesting();
            repo.ResetDatabase("Order_Test_2");
            dbm.Location dbLocation = new dbm.Location { Name = "a" };
            repo.locationRepo.Create(dbLocation);
            repo.SaveChanges();
            int invalidUserId = -1;

            // Ensure the location is valid
            Assert.NotNull(repo.locationRepo.GetById(dbLocation.Id));
            // Ensure the user is invalid
            Assert.Throws<db.InvalidIdException>(() => repo.userRepo.GetById(invalidUserId));

            dbm.Order dbOrder = new dbm.Order
            {
                LocationId = dbLocation.Id,
                UserId = invalidUserId,
                TimePlaced = DateTime.Now,
                TotalPrice = 20.50m
            };

            // Act
            // Create should throw an exception
            Assert.Throws<db.InvalidIdException>(() => repo.orderRepo.Create(dbOrder));


            // Assert
            // New order should not be searchable
            Assert.Throws<db.InvalidIdException>(() => repo.orderRepo.GetById(dbOrder.Id));
        }

    }
}
