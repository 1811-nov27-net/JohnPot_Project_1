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
        public ActionResult Index()
        {
            Sync.OrderProcessor.Initialize(orderRepo.Database);
            Sync.OrderProcessor.SyncFromDatabase();

            return View(view.Mapper.Map(lib.Order.Orders));
        }

        // GET: Order/Details/5
        public ActionResult Details(int id)
        {
            return View(view.Mapper.Map(lib.Order.GetById(id)));
        }

        // GET: Order/Create
        public ActionResult Create(int id)
        {
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
        public ActionResult CreatePizza(view.Order formOrder, string AddPizza, string PlaceOrder)
        {
            lib.Order libOrder;
            if (formOrder.Id != 0)
            {
                libOrder = lib.Order.GetById(formOrder.Id);
            }
            else
                libOrder = view.Mapper.Map(formOrder);

            if (AddPizza != null)
                libOrder.AddPizzaToOrder(formOrder.pizza);
            else if(PlaceOrder != null)
            {
                // Place the order
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

                return RedirectToAction(nameof(Index));
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


                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}