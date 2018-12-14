using Moq;
using db = PizzaStoreData.DataAccess;
using dbm = PizzaStoreData.DataAccess.Models;
using System;
using Xunit;
using Microsoft.EntityFrameworkCore;
using PizzaStoreData.DataAccess.Repositories;

namespace PizzaStoreTesting.Test.RepositoryTesting
{
    public class IngredientRepo
    {
        [Fact]
        public void IngredientDeleteRemovesFromInventoryJunctionTable()
        {
            // Arrange
            RepoTesting repo = new RepoTesting();
            repo.ResetDatabase("Ingredient_Test_1");
            int ingredientId = repo.dbIngredient.Id;
            int locationId   = repo.dbLocation.Id;
            int pizzaId      = repo.dbPizza.PizzaId;

            // Ensure the entities exist
            Assert.NotNull(repo.ingredientRepo.GetById(ingredientId));
            Assert.NotNull(repo.locationRepo.GetById(locationId));
            Assert.NotNull(repo.pizzaRepo.GetById(pizzaId, ingredientId));

            // Act

            repo.ingredientRepo.Delete(repo.dbIngredient);
            repo.SaveChanges();


            // Assert
            // Inventory Junction was removed
            Assert.Throws<db.InvalidIdException>(() => repo.inventoryRepo.GetById(locationId, ingredientId));
            // Ingredient was removed
            Assert.Throws<db.InvalidIdException>(() => repo.ingredientRepo.GetById(ingredientId));

        }
        [Fact]
        public void IngredientDeleteRemovesFromPizzaJunctionTable()
        {
            // Arrange
            RepoTesting repo = new RepoTesting();
            repo.ResetDatabase("Ingredient_Test_2");
            int ingredientId = repo.ingredientRepo.GetById(repo.dbIngredient.Id).Id;
            int pizzaId      = repo.pizzaRepo.GetById(repo.dbPizza.PizzaId, ingredientId).PizzaId;

            // Ensure the entities exist
            Assert.NotNull(repo.ingredientRepo.GetById(ingredientId));
            Assert.NotNull(repo.pizzaRepo.GetById(pizzaId, ingredientId));

            // Act

            repo.ingredientRepo.Delete(repo.dbIngredient);
            repo.SaveChanges();


            // Assert
            // Pizza junction was removed
            Assert.Throws<db.InvalidIdException>(() => repo.pizzaRepo.GetById(pizzaId, ingredientId));
            // Ingredient was removed
            Assert.Throws<db.InvalidIdException>(() => repo.ingredientRepo.GetById(ingredientId));
        }
    }
}
