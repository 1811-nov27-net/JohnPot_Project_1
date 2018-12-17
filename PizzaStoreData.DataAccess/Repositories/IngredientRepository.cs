using PizzaStoreData.DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using lib = PizzaStoreLibrary.library;
using e = PizzaStoreLibrary.library.Exceptions;

namespace PizzaStoreData.DataAccess.Repositories
{
    public class IngredientRepository : lib.IRepository<Ingredient>
    {
        public PizzaStoreDBContext Database { get; set; }

        public IngredientRepository(PizzaStoreDBContext database)
        {
            Database = database ?? throw new ArgumentNullException(nameof(database));
            Database.Database.EnsureCreated();
        }

        public void Create(Ingredient entity)
        {
            Database.Add(entity ?? throw new ArgumentNullException(nameof(entity)));
        }

        public void Delete(Ingredient entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            // Ensure the entity exists within the
            //  database. GetById will throw an 
            //  exception for us if it does not
            entity = GetById(entity.Id);

            // Need to remove ingredient from the InventoryJunction
            //  and PizzaJunction tables
            // Find all InventoryJunctions that contain this ingredient
            List<InventoryJunction> inventoryList = Database.InventoryJunction
                .Where(inv => inv.IngredientId == entity.Id).ToList();
            // For each inventory with ingredient in it, remove
            //  it from the InventoryJunction table
            InventoryJunctionRepository inventoryRepo = new InventoryJunctionRepository(Database);
            foreach (var inventoryJunction in inventoryList)
                inventoryRepo.Delete(inventoryJunction);
            // Now do the same for the PizzaJunction table
            List<PizzaJunction> pizzaList = Database.PizzaJunction
                .Where(pizza => pizza.IngredientId == entity.Id).ToList();
            PizzaJunctionRepository pizzaRepo = new PizzaJunctionRepository(Database);
            foreach (var pizzaJunction in pizzaList)
                pizzaRepo.Delete(pizzaJunction);

            Database.Remove(GetById(entity.Id));
        }

        public Ingredient GetById(params int[] Id)
        {
            // Ingredient has only one Id as PK
            if (Id.Length != 1)
                throw new e.InvalidIdException($"Ingredient: Invalid number of Ids provided. Expected: 1, Actual: {Id.Length}");

            Ingredient ingredient = Database.Ingredient.Find(Id[0]);

            return ingredient ?? throw new e.InvalidIdException($"IngredientId {Id[0]} was not found in the Ingredient table.");
        }
        public List<Ingredient> GetAllIngredients()
        {
            return Database.Ingredient.ToList();
        }

        public void Update(Ingredient entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            Database.Entry(GetById(entity.Id)).CurrentValues.SetValues(entity);
        }

        public void SaveChanges()
        {
            Database.SaveChanges();
        }
    }
}
