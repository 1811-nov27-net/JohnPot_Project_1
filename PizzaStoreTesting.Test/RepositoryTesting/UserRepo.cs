using System;
using Xunit;
using db = PizzaStoreData.DataAccess;
using dbm = PizzaStoreData.DataAccess.Models;

namespace PizzaStoreTesting.Test.RepositoryTesting
{
    public class UserRepo
    {
        [Fact]
        public void UserDeleteRemovesOrderJunctionAndOrderThatRefersToIt()
        {
            // Arrange
            RepoTesting repo = new RepoTesting();
            repo.ResetDatabase("User_Test_1");
            int orderId = repo.dbOrder.Id;
            int pizzaId = repo.dbPizza.PizzaId;
            int ingredientId = repo.dbIngredient.Id;
            int userId = repo.dbUser.Id;

            // Ensure the entities exist
            Assert.NotNull(repo.orderRepo.GetById(orderId));
            Assert.NotNull(repo.pizzaRepo.GetById(pizzaId, ingredientId));
            Assert.NotNull(repo.orderJunctionRepo.GetById(orderId, pizzaId));

            // Act

            repo.userRepo.Delete(repo.dbUser);
            repo.SaveChanges();


            // Assert
            // Order was removed
            Assert.Throws<db.InvalidIdException>(() => repo.orderRepo.GetById(orderId));
            // Order junction was removed              
            Assert.Throws<db.InvalidIdException>(() => repo.orderJunctionRepo.GetById(orderId, pizzaId));
            // User was removed              
            Assert.Throws<db.InvalidIdException>(() => repo.userRepo.GetById(userId));

        }
        [Fact]
        public void UserCreateSucceedsWithValidDefaultLocation()
        {
            // Arrange
            RepoTesting repo = new RepoTesting();
            repo.ResetDatabase("User_Test_2");

            dbm.Location dbLocation = new dbm.Location { Name = "a" };
            repo.locationRepo.Create(dbLocation);
            repo.SaveChanges();

            // Ensure the location exists
            Assert.NotNull(repo.locationRepo.GetById(dbLocation.Id));

            dbm.User dbUser = new dbm.User
            {
                FirstName = "John",
                LastName = "Pot",
                DefaultLocationId = dbLocation.Id
            };

            // Act
            repo.userRepo.Create(dbUser);

            // Assert
            // User should now exist within the database
            Assert.NotNull(repo.userRepo.GetById(dbUser.Id));
        }
        [Fact]
        public void UserCreateFailsWithInvalidDefaultLocationId()
        {
            // Arrange
            RepoTesting repo = new RepoTesting();
            repo.ResetDatabase("User_Test_3");

            int invalidLocationId = -1;

            // Ensure the locationId is invalid
            Assert.Throws<db.InvalidIdException>(() => repo.locationRepo.GetById(invalidLocationId));

            dbm.User dbUser = new dbm.User
            {
                FirstName = "John",
                LastName = "Pot",
                DefaultLocationId = invalidLocationId
            };

            // Act
            // Create should throw exception
            Assert.Throws<db.InvalidIdException>(() => repo.userRepo.Create(dbUser));

            // Assert
            // User should not be findable
            Assert.Throws<db.InvalidIdException>(() => repo.userRepo.GetById(dbUser.Id));
        }

    }
}
