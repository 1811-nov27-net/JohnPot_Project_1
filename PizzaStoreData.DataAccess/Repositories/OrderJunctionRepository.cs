using PizzaStoreData.DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Text;
using lib = PizzaStoreLibrary.library;

namespace PizzaStoreData.DataAccess.Repositories
{
    public class OrderJunctionRepository : lib.IRepository<OrderJunction>
    {
        private PizzaStoreDBContext Database { get; set; }
        public OrderJunctionRepository(PizzaStoreDBContext database)
        {
            Database = database ?? throw new ArgumentNullException(nameof(database));
        }

        public void Create(OrderJunction entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            // Ensure the Order / Pizza exist
            //  before trying to create the junction
            // (GetById will throw if not)
            OrderRepository orderRepo = new OrderRepository(Database);
            orderRepo.GetById(entity.OrderId);
            PizzaJunctionRepository pizzaRepo = new PizzaJunctionRepository(Database);
            pizzaRepo.GetById(entity.PizzaId);

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
                throw new InvalidIdException($"OrderJunction: Invalid number of Ids provided. Expected: 1, Actual: {Id.Length}");

            OrderJunction orderJunction = Database.OrderJunction.Find(Id);

            return orderJunction ?? throw new InvalidIdException($"OrderJunction: OrderId {Id[0]} + PizzaId {Id[1]} was not found in the OrderJunction table.");
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
    }
}
