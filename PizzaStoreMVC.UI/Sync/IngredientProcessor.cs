using System;
using System.Collections.Generic;
using System.Linq;
using lib = PizzaStoreLibrary.library.Models;
using dbr = PizzaStoreData.DataAccess.Repositories;
using db = PizzaStoreData.DataAccess.Models;
using e = PizzaStoreLibrary.library.Exceptions;

namespace PizzaStoreMVC.UI.Sync
{
    public static class IngredientProcessor
    {
        private static PizzaStoreData.DataAccess.PizzaStoreDBContext Database;
        private static dbr.IngredientRepository ingredientRepo;


        public static void Initialize(PizzaStoreData.DataAccess.PizzaStoreDBContext database)
        {
            Database = database ?? throw new ArgumentNullException(nameof(database));
            ingredientRepo = new dbr.IngredientRepository(Database);
        }

        public static void SyncFromDatabase()
        {
            lib.Ingredient.Ingredients.Clear();
            List<db.Ingredient> ingredients = ingredientRepo.GetAllIngredients();
            foreach(var ingredient in ingredients)
            {
                lib.Ingredient i = new lib.Ingredient
                {
                    Id = ingredient.Id,
                    Name = ingredient.Name,
                    Price = ingredient.Price
                };
            }
        }
        

    }
}
