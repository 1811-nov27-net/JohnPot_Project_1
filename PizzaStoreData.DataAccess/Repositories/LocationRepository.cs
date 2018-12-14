using PizzaStoreData.DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using lib = PizzaStoreLibrary.library;

namespace PizzaStoreData.DataAccess.Repositories
{
    public class LocationRepository : lib.IRepository<Location>
    {
        private PizzaStoreDBContext Database { get; set; }

        public LocationRepository(PizzaStoreDBContext database)
        {
            Database = database ?? throw new ArgumentNullException(nameof(database));
        }

        public void Create(Location entity)
        {
            Database.Add(entity ?? throw new ArgumentNullException(nameof(entity)));
        }

        public void Delete(Location entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            // Ensure the location exists. 
            //  GetById will throw an exception for us
            entity = GetById(entity.Id);

            // Remove all orders that were placed at this location 
            //  from the order table
            List<Order> orderList = Database.Order
                .Where(o => o.LocationId == entity.Id).ToList();
            // Using OrderRepo for cascading removal
            OrderRepository orderRepo = new OrderRepository(Database);
            foreach (var order in orderList)
                orderRepo.Delete(order);
            // Reset any users default locations that refer to
            //  this location.
            List<User> userList = Database.User
                .Where(u => u.DefaultLocationId == entity.Id).ToList();
            foreach (var user in userList)
            {
                // Reset the default location
                user.DefaultLocationId = null;
                // Update the user in the User table
                UserRepository userRepo = new UserRepository(Database);
                userRepo.Update(user);
            }
            // Need to remove all InventoryJunction entries
            //  that refer to this location
            List<InventoryJunction> inventoryList = Database.InventoryJunction
                .Where(inv => inv.LocationId == entity.Id).ToList();
            // TODO: Use InventoryJunctionRepo for removal
            foreach (var inventoryJunction in inventoryList)
                Database.InventoryJunction.Remove(inventoryJunction);

            // We can now safely remove the location
            Database.Remove(entity);
        }

        public Location GetById(params int[] Id)
        {
            // Location only has one Id as PK
            if (Id.Length != 1)
                throw new InvalidIdException($"Location: Invalid number of Ids provided. Expected: 1, Actual: {Id.Length}");

            Location location = Database.Location.Find(Id);

            return location ?? throw new InvalidIdException($"LocationId {Id[0]} was not found in the Location table.");
        }

        public void Update(Location entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            Database.Entry(GetById(entity.Id)).CurrentValues.SetValues(entity);
        }
    }
}
