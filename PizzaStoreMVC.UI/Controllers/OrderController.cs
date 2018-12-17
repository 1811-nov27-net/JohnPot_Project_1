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
            return View(lib.Order.Orders);
        }

        // GET: Order/Details/5
        public ActionResult Details(int id)
        {
            return View(lib.Order.GetById(id));
        }

        // GET: Order/Create
        public ActionResult Create()
        {
            return View();
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
        
        // GET: Order/Delete/5
        public ActionResult Delete(int id)
        {
            return View(lib.Order.GetById(id));
        }

        // POST: Order/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                lib.Order libOrder = lib.Order.GetById(id);
                lib.Order.Orders.Remove(libOrder);
                db.Order dbOrder;
                List<db.OrderJunction> orderJunctionList;
                List<db.PizzaJunction> pizzaJunctionList;
                dbOrder = db.Mapper.Map(libOrder, out orderJunctionList, out pizzaJunctionList);

                foreach (var orderJunction in orderJunctionList)
                {
                    orderRepo.orderJunctionRepo.Delete(orderJunction);
                }
                foreach (var pizzaJunction in pizzaJunctionList)
                {
                    orderRepo.pizzaJunctionRepo.Delete(pizzaJunction);
                }
                orderRepo.Delete(dbOrder);
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