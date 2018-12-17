using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using lib = PizzaStoreLibrary.library.Models;
using dbr = PizzaStoreData.DataAccess.Repositories;
using db = PizzaStoreData.DataAccess.Models;
using e = PizzaStoreLibrary.library.Exceptions;
using PizzaStoreData.DataAccess;

namespace PizzaStoreMVC.UI.Sync
{
    public class LocationProcessor
    {
        private static PizzaStoreData.DataAccess.PizzaStoreDBContext Database;
        private static dbr.LocationRepository locationRepo;

        public static void Initialize(PizzaStoreData.DataAccess.PizzaStoreDBContext database)
        {
            Database = database ?? throw new ArgumentNullException(nameof(database));
            locationRepo = new dbr.LocationRepository(Database);
        }

        public static void SyncFromDatabase()
        {
            lib.Location.Locations.Clear();
            List<db.Location> locations = locationRepo.GetAllLocations();
            foreach (var location in locations)
            {
                db.Mapper.Map(location);
            }
        }
    }
}
