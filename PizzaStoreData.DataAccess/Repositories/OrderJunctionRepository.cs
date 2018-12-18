using PizzaStoreData.DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using lib = PizzaStoreLibrary.library;
using e = PizzaStoreLibrary.library.Exceptions;

namespace PizzaStoreData.DataAccess.Repositories
{
    public class OrderJunctionRepository : lib.IRepository<OrderJunction>
    {
        private PizzaStoreDBContext Database { get; set; }
        public OrderJunctionRepository(PizzaStoreDBContext database)
        {
            Database = database ?? throw new ArgumentNullException(nameof(database));
            Database.Database.EnsureCreated();
        }

        public void Create(OrderJunction entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            // Ensure the Order exists before trying to create the junction
            // (GetById will throw if not)
            //OrderRepository orderRepo = new OrderRepository(Database);
            //orderRepo.GetById(entity.OrderId);
            // Make sure the PizzaJunction also exists
            //List<PizzaJunction> pizzaList = Database.PizzaJunction
            //    .Where(p => p.PizzaId == entity.PizzaId).ToList();
            //if (pizzaList.Count == 0)
              //  throw new e.InvalidIdException("OrderJunctions: Could not create due to invalid PizzaId.");

            // Location / Ingredients are valid. Can successfully create
            //  the inventory junction
            Database.Add(entity);
        }

        public void Delete(OrderJunction entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            // Ensure the OrderJunction exists within the database
            //  GetById will throw an exception if not
            entity = GetById(entity.OrderId, entity.PizzaId);

            Database.Remove(entity);
        }

        public OrderJunction GetById(params int[] Id)
        {
            // OrderJunction PK is composed of 2 Ids:
            //  OrderId and PizzaId
            if (Id.Length != 2)
                throw new e.InvalidIdException($"OrderJunction: Invalid number of Ids provided. Expected: 2, Actual: {Id.Length}");

            OrderJunction orderJunction = Database.OrderJunction.Find(Id[0], Id[1]);

            return orderJunction ?? throw new e.InvalidIdException($"OrderJunction: OrderId {Id[0]} + PizzaId {Id[1]} was not found in the OrderJunction table.");
        }

        public void Update(OrderJunction entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            // Ensure the OrderJunction exists within the database
            //  GetById will throw an exception if not
            entity = GetById(entity.OrderId, entity.PizzaId);

            Database.Entry(entity).CurrentValues.SetValues(entity);
        }

        public void SaveChanges()
        {
            Database.SaveChanges();
        }

        public List<OrderJunction> GetAllOrderJunctions()
        {
            return Database.OrderJunction.ToList();
        }
    }
}
