using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using lib = PizzaStoreLibrary.library.Models;
using db = PizzaStoreData.DataAccess.Models;
using dbr = PizzaStoreData.DataAccess.Repositories;
using Microsoft.EntityFrameworkCore;
using view = PizzaStoreMVC.UI.Models;
using System.Data;
using e = PizzaStoreLibrary.library.Exceptions;

namespace PizzaStoreMVC.UI.Controllers
{
    public class OrderController : Controller
    {
        public dbr.OrderRepository orderRepo;

        public OrderController(dbr.OrderRepository repo)
        {
            orderRepo = repo;
        }

        // GET: Order
        public ActionResult Index(bool resync)
        {

            if(resync || lib.Order.OrderHasBeenPlace)
            {
                
                Sync.OrderProcessor.Initialize(orderRepo.Database);
                Sync.OrderProcessor.SyncFromDatabase();
            }

            return View(view.Mapper.Map(lib.Order.Orders));
        }

        public ActionResult Sort(int sortmethod)
        {
            switch(sortmethod)
            {
                case 1:
                    {
                        lib.Order.Orders = lib.Order.Orders.OrderBy(o => o.TimePlaced).ToList();
                        break;
                    }
                case 2:
                    {
                        lib.Order.Orders = lib.Order.Orders.OrderByDescending(o => o.TimePlaced).ToList();
                        break;
                    }
                case 3:
                    {
                        lib.Order.Orders = lib.Order.Orders.OrderBy(o => o.TotalPrice).ToList();
                        break;
                    }
                case 4:
                    {
                        lib.Order.Orders = lib.Order.Orders.OrderByDescending(o => o.TotalPrice).ToList();
                        break;
                    }
            }

            bool resync = false;
            return RedirectToAction(nameof(Index), new { resync });
        }

        // GET: Order/Details/5
        public ActionResult Details(int id)
        {
            return View(view.Mapper.Map(lib.Order.GetById(id)));
        }

        // GET: Order/Create
        public ActionResult Create(int id)
        {
            //int storeId = Int32.Parse((string)ViewData["StoreId"]);

            try
            {
                lib.Order libOrder = lib.Order.GetById(id);
                view.Order formOrder = view.Mapper.Map(libOrder);

                return View(formOrder);

            }
            catch
            {
                return View(new view.Order());
            }

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreatePizza(view.Order formOrder, string AddPizza, string PlaceOrder, string Default, string Suggest)
        {
            lib.Order libOrder;
            if (formOrder.Id != 0)
            {
                libOrder = lib.Order.GetById(formOrder.Id);
            }
            else
                libOrder = view.Mapper.Map(formOrder);
            if(Suggest != null)
            {
                lib.Order temp = libOrder;
                libOrder = lib.Order.Orders.Where(o => o.UserId == libOrder.UserId).OrderByDescending(o => o.TimePlaced).FirstOrDefault();
                if (libOrder != null)
                {
                    lib.Order.Orders.Remove(libOrder);
                    lib.Order o = new lib.Order
                    {
                        LocationId = libOrder.LocationId,
                        UserId = libOrder.UserId
                    };
                    foreach(var pizza in libOrder.Pizzas)
                    {
                            o.AddPizzaToOrder(pizza.Ingredients.Select(i => i.Name).ToList().ToArray());
                    }

                    return RedirectToAction(nameof(Create), new { o.Id });

                }
                else
                    libOrder = temp;
            }
            if (AddPizza != null)
            {
                if(libOrder.Pizzas.Count < 12 && libOrder.TotalPrice + formOrder.pizza.Price <= 500)
                {
                    // 2 hour check
                    lib.Order mostRecentOrder = lib.Order.Orders
                        .Where(o => o.UserId == libOrder.UserId)
                        .OrderByDescending(o => o.TimePlaced)
                        .FirstOrDefault();
                    //if(mostRecentOrder != null)
                      //  if((DateTime.Now - mostRecentOrder.TimePlaced).Hours > 2)
                            libOrder.AddPizzaToOrder(formOrder.pizza);
                }
            }
            else if(PlaceOrder != null || Default != null)
            {
                if(Default != null)
                {
                    lib.User libUser = lib.User.GetById(libOrder.UserId);
                    if (libUser.DefaultLocationId != null)
                        libOrder.LocationId = (int)libUser.DefaultLocationId;
                }

                // Place the order
                try
                {
                    // Attempt to place the order
                    // Get all the ingredients required to place the order
                    List<string> ingredients = new List<string>();
                    foreach(var pizza in libOrder.Pizzas)
                    {
                        foreach (var ingredient in pizza.Ingredients)
                            ingredients.Add(ingredient.Name);
                    }
                    var inventory = ingredients.Select(s => new KeyValuePair<string, int>(s, ingredients.Where(s2 => s2 == s).Count())).ToList();
                    lib.Location.GetById(libOrder.LocationId).UpdateInventory(inventory);
                }
                catch(e.InsufficientIngredientException)
                {

                }
                lib.Order.OrderHasBeenPlace = true;
                libOrder.TimePlaced = DateTime.Now;
                db.Order dbOrder;
                List<db.OrderJunction> orderJunctionList;
                List<db.PizzaJunction> pizzaJunctionList;
                dbOrder = db.Mapper.Map(libOrder, out orderJunctionList, out pizzaJunctionList);

                foreach (var pizzaJunction in pizzaJunctionList)
                {
                    orderRepo.pizzaJunctionRepo.Create(pizzaJunction);
                }
                foreach (var orderJunction in orderJunctionList)
                {
                    orderRepo.orderJunctionRepo.Create(orderJunction);
                }
                orderRepo.Create(dbOrder);
                orderRepo.SaveChanges();

                bool resync = false;
                return RedirectToAction(nameof(Index), new { resync });
            }

            return RedirectToAction(nameof(Create), new { libOrder.Id });
        }


        // POST: Order/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(lib.Order formOrder)
        {
            try
            {
                lib.Order libOrder = lib.Order.GetById(formOrder.Id);

                db.Order dbOrder;
                List<db.OrderJunction> orderJunctionList;
                List<db.PizzaJunction> pizzaJunctionList;
                dbOrder = db.Mapper.Map(libOrder, out orderJunctionList, out pizzaJunctionList);

                foreach(var orderJunction in orderJunctionList)
                {
                    orderRepo.orderJunctionRepo.Create(orderJunction);
                }
                foreach(var pizzaJunction in pizzaJunctionList)
                {
                    orderRepo.pizzaJunctionRepo.Create(pizzaJunction);
                }
                orderRepo.Create(dbOrder);
                orderRepo.SaveChanges();

                bool resync = false;
                return RedirectToAction(nameof(Index),  new { resync });
            }
            catch
            {
                return View();
            }
        }
    }
}