using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using lib = PizzaStoreLibrary.library.Models;
using dbr = PizzaStoreData.DataAccess.Repositories;
using db = PizzaStoreData.DataAccess.Models;
using e = PizzaStoreLibrary.library.Exceptions;

namespace PizzaStoreMVC.UI.Sync
{
    public class UserProcessor
    {
        private static PizzaStoreData.DataAccess.PizzaStoreDBContext Database;
        private static dbr.UserRepository userRepo;


        public static void Initialize(PizzaStoreData.DataAccess.PizzaStoreDBContext database)
        {
            Database = database ?? throw new ArgumentNullException(nameof(database));
            userRepo = new dbr.UserRepository(Database);
        }

        public static void SyncFromDatabase()
        {
            lib.User.Users.Clear();
            List<db.User> users = userRepo.GetAllUsers();
            foreach (var dbUser in users)
            {
                lib.User libUser = new lib.User
                {
                    Id = dbUser.Id,
                    FirstName = dbUser.FirstName,
                    LastName = dbUser.LastName,
                    DefaultLocationId = dbUser.DefaultLocationId
                };
            }
        }
    }
}
