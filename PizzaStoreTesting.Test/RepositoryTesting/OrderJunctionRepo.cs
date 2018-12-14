using System;
using Xunit;
using db = PizzaStoreData.DataAccess;
using dbm = PizzaStoreData.DataAccess.Models;

namespace PizzaStoreTesting.Test.RepositoryTesting
{
    public class OrderJunctionRepo
    {
        [Fact]
        public void CreatingNewOrderJunctionSucceedsWithValidOrderIdandPizzaId()
        {
            // Arrange
            RepoTesting repo = new RepoTesting();
            repo.ResetDatabase("OrderJunction_Test_1");
            dbm.PizzaJunction dbPizza = new dbm.PizzaJunction { PizzaId = dbm.PizzaJunction.GetNewId(), Count = 1, IngredientId = repo.dbIngredient.Id };
            repo.pizzaRepo.Create(dbPizza);
            repo.SaveChanges();

            // Ensure the required entities exist
            Assert.NotNull(repo.pizzaRepo.GetById(dbPizza.PizzaId, dbPizza.IngredientId));

            dbm.OrderJunction dbOrderJunction = new dbm.OrderJunction
            { OrderId = repo.dbOrder.Id, PizzaId = dbPizza.PizzaId };

            // Act
            // Create the new order junction
            repo.orderJunctionRepo.Create(dbOrderJunction);

            // Assert
            // Searching for this orderJunction should now succeed
            Assert.NotNull(repo.orderJunctionRepo.GetById(repo.dbOrder.Id, dbPizza.PizzaId));
        }
        [Fact]
        public void CreatingNewOrderJunctionFailsWithInvalidOrderId()
        {
            // Arrange
            RepoTesting repo = new RepoTesting();
            repo.ResetDatabase("OrderJunction_Test_2");
            int invalidOrderId = -1;

            // Ensure the required entities exist
            Assert.NotNull(repo.pizzaRepo.GetById(repo.dbPizza.PizzaId, repo.dbPizza.IngredientId));

            dbm.OrderJunction dbOrderJunction = new dbm.OrderJunction
            { OrderId = invalidOrderId, PizzaId = repo.dbPizza.PizzaId };

            // Act
            // Create the new order junction
            Assert.Throws<db.InvalidIdException>(() => repo.orderJunctionRepo.Create(dbOrderJunction));

            // Assert
            // Searching for this orderJunction should fail
            Assert.Throws<db.InvalidIdException>(() => repo.orderJunctionRepo.GetById(invalidOrderId, repo.dbPizza.PizzaId));
        }
        [Fact]
        public void CreatingNewOrderJunctionFailsWithInvalidPizzaId()
        {
            // Arrange
            RepoTesting repo = new RepoTesting();
            repo.ResetDatabase("OrderJunction_Test_3");
            int invalidPizzaId = -1;

            // Ensure the required entities exist
            Assert.NotNull(repo.orderRepo.GetById(repo.dbOrder.Id));

            dbm.OrderJunction dbOrderJunction = new dbm.OrderJunction
            { OrderId = repo.dbOrder.Id, PizzaId = invalidPizzaId };

            // Act
            // Create the new order junction
            Assert.Throws<db.InvalidIdException>(() => repo.orderJunctionRepo.Create(dbOrderJunction));

            // Assert
            // Searching for this orderJunction should fail
            Assert.Throws<db.InvalidIdException>(() => repo.orderJunctionRepo.GetById(repo.dbOrder.Id, invalidPizzaId));
        }
    }
}
