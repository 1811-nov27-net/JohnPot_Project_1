using System;
using Xunit;
using db = PizzaStoreData.DataAccess;
using dbm = PizzaStoreData.DataAccess.Models;

namespace PizzaStoreTesting.Test.RepositoryTesting
{
    public class PizzaJunctionRepo
    {
        [Fact]
        public void CreatingNewPizzaJunctionSucceedsWithValidPizzaIdAndIngredientId()
        {
            // Arrange
            RepoTesting repo = new RepoTesting();
            repo.ResetDatabase("PizzaJunction_Test_1");
            dbm.PizzaJunction dbPizza = new dbm.PizzaJunction{ IngredientId = repo.dbIngredient.Id, Count = 1 };
            repo.pizzaRepo.Create(dbPizza);

            // Ensure the required entities exist
            Assert.NotNull(repo.pizzaRepo.GetById(dbPizza.PizzaId, dbPizza.IngredientId));
            Assert.NotNull(repo.ingredientRepo.GetById(repo.dbIngredient.Id));

            // Act
            // Create the PizzaJuncton
            repo.pizzaRepo.Create(dbPizza);

            // Assert
            // Searching for this PizzaJunction should now succeed
            Assert.NotNull(repo.pizzaRepo.GetById(dbPizza.PizzaId, repo.dbIngredient.Id));
        }
        [Fact]
        public void CreatingNewPizzaJunctionFailsWithInvalidIngredientId()
        {
            // Arrange
            RepoTesting repo = new RepoTesting();
            repo.ResetDatabase("PizzaJunction_Test_2");
            int invalidIngredientId = -1;
            dbm.PizzaJunction dbPizza = new dbm.PizzaJunction { IngredientId = invalidIngredientId, Count = 1 };
            
            // Ensure the invalid ingredientId is in fact invalid
            Assert.Throws<db.InvalidIdException>(() => repo.ingredientRepo.GetById(invalidIngredientId));

            // Act
            // Creating the PizzaJunction should not work. 
            Assert.Throws<db.InvalidIdException>(() => repo.pizzaRepo.Create(dbPizza));

            // Assert
            // Searching for this PizzaJunction should fail
            Assert.Throws<db.InvalidIdException>(() => repo.pizzaRepo.GetById(dbPizza.PizzaId, repo.dbIngredient.Id));
        }

        [Fact]
        public void PizzaJunctionDeleteRemovesFromOrderTable()
        {
            // Arrange
            RepoTesting repo = new RepoTesting();
            repo.ResetDatabase("PizzaJuntion_Test_3");
            db.Models.OrderJunction dbOrderJunction = repo.dbOrderJunction;
            int orderId = dbOrderJunction.OrderId;
            int pizzaId = dbOrderJunction.PizzaId;
            int ingredientId = repo.dbIngredient.Id;

            // Ensure the entities exist
            // order junction
            Assert.NotNull(repo.orderJunctionRepo.GetById(dbOrderJunction.OrderId, dbOrderJunction.PizzaId));
            // Pizza junction
            Assert.NotNull(repo.pizzaRepo.GetById(pizzaId, ingredientId));

            // Act
            repo.pizzaRepo.Delete(repo.dbPizza);
            repo.SaveChanges();


            // Assert
            // Order junction was removed
            Assert.Throws<db.InvalidIdException>(() => repo.orderJunctionRepo.GetById(dbOrderJunction.OrderId, dbOrderJunction.PizzaId));
            // Pizza junction was removed
            Assert.Throws<db.InvalidIdException>(() => repo.pizzaRepo.GetById(pizzaId, ingredientId));

        }

    }
}
