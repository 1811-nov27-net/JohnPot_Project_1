using PizzaStoreData.DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using lib = PizzaStoreLibrary.library;
using e = PizzaStoreLibrary.library.Exceptions;

namespace PizzaStoreData.DataAccess.Repositories
{
    public class OrderRepository : lib.IRepository<Order>
    {
        private PizzaStoreDBContext Database { get; set; }
        public OrderJunctionRepository orderJunctionRepo;
        public PizzaJunctionRepository pizzaJunctionRepo;

        public OrderRepository(PizzaStoreDBContext database)
        {
            Database = database ?? throw new ArgumentNullException(nameof(database));
            orderJunctionRepo = new OrderJunctionRepository(database);
            pizzaJunctionRepo = new PizzaJunctionRepository(database);

            Database.Database.EnsureCreated();
        }

        public void Create(Order entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            // Ensure the location and user of this 
            //  order are valid. (GetById will throw if not)
            LocationRepository locationRepo = new LocationRepository(Database);
            locationRepo.GetById(entity.LocationId);
            UserRepository userRepo = new UserRepository(Database);
            userRepo.GetById(entity.UserId);

            Database.Add(entity);
        }

        public void Delete(Order entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            // Ensure the entity exists within the
            //  database. GetById will throw an
            //  exception for us if it does not.
            entity = GetById(entity.Id);

            // Need to remove all OrderJunctions that 
            //  refer to this table
            List<OrderJunction> orderJunctionList = Database.OrderJunction
                .Where(o => o.OrderId == entity.Id).ToList();
            OrderJunctionRepository orderJunctionRepo = new OrderJunctionRepository(Database);
            foreach (var orderJunction in orderJunctionList)
                orderJunctionRepo.Delete(orderJunction);

            Database.Remove(entity);
        }

        public Order GetById(params int[] Id)
        {
            // Order only has one Id as PK
            if (Id.Length != 1)
                throw new e.InvalidIdException($"Order: Invalid number of Ids provided. Expected: 1, Actual: {Id.Length}");

            Order order = Database.Order.Find(Id[0]);

            return order ?? throw new e.InvalidIdException($"OrderId {Id[0]} was not found in the Order table.");
        }
        public List<Order> GetAllOrders()
        {
            return Database.Order.ToList();
        }

        public void Update(Order entity)
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
