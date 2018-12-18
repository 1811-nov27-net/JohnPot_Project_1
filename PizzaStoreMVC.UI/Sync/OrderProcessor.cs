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
    public class OrderProcessor
    {
        private static PizzaStoreData.DataAccess.PizzaStoreDBContext Database;
        private static dbr.OrderRepository orderRepo;


        public static void Initialize(PizzaStoreData.DataAccess.PizzaStoreDBContext database)
        {
            Database = database ?? throw new ArgumentNullException(nameof(database));
            orderRepo = new dbr.OrderRepository(Database);
        }

        public static void SyncFromDatabase()
        {
            // OrderJunction and PizzaJunction tables need to be
            //  synced before this can be synced.
            lib.Order.Orders.Clear();
            List<db.Order> orders = orderRepo.GetAllOrders();
            foreach (var dbOrder in orders)
            {
                db.Mapper.Map(dbOrder);
            }
            lib.Order.OrderHasBeenPlace = false;
        }
    }
}
