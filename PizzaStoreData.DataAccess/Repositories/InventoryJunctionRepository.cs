using PizzaStoreData.DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Text;
using lib = PizzaStoreLibrary.library;
using e = PizzaStoreLibrary.library.Exceptions;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace PizzaStoreData.DataAccess.Repositories
{
    public class InventoryJunctionRepository : lib.IRepository<InventoryJunction>
    {
        private PizzaStoreDBContext Database { get; set; }
        public InventoryJunctionRepository(PizzaStoreDBContext database)
        {
            Database = database ?? throw new ArgumentNullException(nameof(database));
            Database.Database.EnsureCreated();
        }

        public void Create(InventoryJunction entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            // Ensure the Location / Ingredient exist
            //  before trying to create the junction
            // (GetById will throw if not)
            LocationRepository locationRepo = new LocationRepository(Database);
            locationRepo.GetById(entity.LocationId);
            IngredientRepository ingredientRepo = new IngredientRepository(Database);
            ingredientRepo.GetById(entity.IngredientId);

            // Location / Ingredients are valid. Can successfully create
            //  the inventory junction
            Database.Add(entity);
        }

        public void Delete(InventoryJunction entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            // Ensure the InventoryJunction exists within the database
            //  GetById will throw an exception if not
            entity = GetById(entity.LocationId, entity.IngredientId);

            Database.Remove(entity);
        }

        public InventoryJunction GetById(params int[] Id)
        {
            // InventoryJunction PK is composed of 2 Ids:
            //  LocationId and IngredientId 
            if (Id.Length != 2)
                throw new e.InvalidIdException($"InventoryJunction: Invalid number of Ids provided. Expected: 2, Actual: {Id.Length}");
            
            InventoryJunction inventoryJunction = Database.InventoryJunction.Find(Id[0], Id[1]);

            return inventoryJunction ?? throw new e.InvalidIdException($"LocationId {Id[0]} + IngredientId {Id[1]} was not found in the InventoryJunction table.");
        }
        public List<InventoryJunction> GetAllInventoryJunctions()
        {
            return Database.InventoryJunction.ToList();
        }

        public void Update(InventoryJunction entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            Database.Entry(GetById(entity.LocationId, entity.IngredientId)).CurrentValues.SetValues(entity);
        }

        public void SaveChanges()
        {
            Database.SaveChanges();
        }
    }
}
