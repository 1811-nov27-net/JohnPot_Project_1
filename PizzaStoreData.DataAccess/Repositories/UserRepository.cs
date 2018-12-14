using PizzaStoreData.DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using lib = PizzaStoreLibrary.library;

namespace PizzaStoreData.DataAccess.Repositories
{
    public class UserRepository : lib.IRepository<User>
    {
        private PizzaStoreDBContext Database { get; set; }

        public UserRepository(PizzaStoreDBContext database)
        {
            Database = database ?? throw new ArgumentNullException(nameof(database));
            Database.Database.EnsureCreated();
        }

        public void Create(User entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            // Ensure the default location exists within the database
            if(entity.DefaultLocationId != null)
            {
                LocationRepository locationRepo = new LocationRepository(Database);
                // GetById will throw if location is invalid
                locationRepo.GetById((int)entity.DefaultLocationId);
            }

            Database.Add(entity);
        }

        public void Delete(User entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            // Ensure the entity exists within the
            //  database. GetById will throw an
            //  exception for us if it does not.
            entity = GetById(entity.Id);
            // Need to remove all orders that were placed by this
            //  user from the database.
            List<Order> orderList = Database.Order
                .Where(o => o.UserId == entity.Id).ToList();
            // Create order repo to deal with removal
            //  for cascaded deletion. (We also need 
            //  to remove any orderJunctions refering to each order)
            OrderRepository orderRepo = new OrderRepository(Database);
            foreach (var order in orderList)
                orderRepo.Delete(order);

            Database.Remove(entity);
        }

        public User GetById(params int[] Id)
        {
            // User has only 1 Id as PK
            if (Id.Length != 1)
                throw new InvalidIdException($"User: Invalid number of Ids provided. Expected: 1, Actual: {Id.Length}");

            User user = Database.User.Find(Id[0]);

            return user ?? throw new InvalidIdException($"UserId {Id[0]} was not found in the User table.");
        }

        public void Update(User entity)
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
