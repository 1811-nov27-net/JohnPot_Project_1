using PizzaStoreData.DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using lib = PizzaStoreLibrary.library;

namespace PizzaStoreData.DataAccess.Repositories
{
    public class OrderRepository : lib.IRepository<Order>
    {
        private PizzaStoreDBContext Database { get; set; }

        public OrderRepository(PizzaStoreDBContext database)
        {
            Database = database ?? throw new ArgumentNullException(nameof(database));
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
            // TODO: Use order junction repo to remove from table
            List<OrderJunction> orderJunctionList = Database.OrderJunction
                .Where(o => o.OrderId == entity.Id).ToList();
            foreach (var orderJunction in orderJunctionList)
                Database.OrderJunction.Remove(orderJunction);

            Database.Remove(entity);
        }

        public Order GetById(params int[] Id)
        {
            // Order only has one Id as PK
            if (Id.Length != 1)
                throw new InvalidIdException($"Order: Invalid number of Ids provided. Expected: 1, Actual: {Id.Length}");

            Order order = Database.Order.Find(Id);

            return order ?? throw new InvalidIdException($"OrderId {Id[0]} was not found in the Order table.");
        }

        public void Update(Order entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            Database.Entry(GetById(entity.Id)).CurrentValues.SetValues(entity);
        }
    }
}
