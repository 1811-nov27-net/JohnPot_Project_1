using PizzaStoreData.DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using lib = PizzaStoreLibrary.library;
using e = PizzaStoreLibrary.library.Exceptions;

namespace PizzaStoreData.DataAccess.Repositories
{
    public class PizzaJunctionRepository : lib.IRepository<PizzaJunction>
    {
        private PizzaStoreDBContext Database { get; set; }
        public PizzaJunctionRepository(PizzaStoreDBContext database)
        {
            Database = database ?? throw new ArgumentNullException(nameof(database));
            Database.Database.EnsureCreated();
        }

        public void Create(PizzaJunction entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            // Ensure that ingredient exists before trying to create
            //  the junction including it
            IngredientRepository ingredientRepo = new IngredientRepository(Database);
            // GetById will throw an exception if not
            ingredientRepo.GetById(entity.IngredientId);

            // Location / Ingredients are valid. Can successfully create
            //  the inventory junction
            Database.Add(entity);
        }

        public void Delete(PizzaJunction entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            // Need to remove any OrderJunction entities that
            //  reference this PizzaJunction
            List<OrderJunction> orderJunctions = Database.OrderJunction
                .Where(o => o.PizzaId == entity.PizzaId).ToList();
            OrderJunctionRepository orderJunctionRepo = new OrderJunctionRepository(Database);
            foreach (var orderJunction in orderJunctions)
                orderJunctionRepo.Delete(orderJunction);

            // Ensure the PizzaJunction exists within the database
            //  GetById will throw an exception if not
            entity = GetById(entity.PizzaId, entity.IngredientId);

            Database.Remove(entity);
        }

        public PizzaJunction GetById(params int[] Id)
        {
            // PizzaJunction PK is composed of 2 Ids:
            //  PizzaId and IngredientId
            if (Id.Length != 2)
                throw new e.InvalidIdException($"PizzaJunction: Invalid number of Ids provided. Expected: 2, Actual: {Id.Length}");

            PizzaJunction pizzaJunction = Database.PizzaJunction.Find(Id[0], Id[1]);

            return pizzaJunction ?? throw new e.InvalidIdException($"Pizza: Id {Id[0]} + IngredientId {Id[1]} was not found in the PizzaJunction table.");
        }

        public void Update(PizzaJunction entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            // Ensure the PizzaJunction exists within the database
            //  GetById will throw an exception if not
            entity = GetById(entity.PizzaId, entity.IngredientId);

            Database.Entry(entity).CurrentValues.SetValues(entity);
        }

        public void SaveChanges()
        {
            Database.SaveChanges();
        }

        public List<PizzaJunction> GetAllPizzaJunctions()
        {
            return Database.PizzaJunction.ToList();
        }
    }
}
