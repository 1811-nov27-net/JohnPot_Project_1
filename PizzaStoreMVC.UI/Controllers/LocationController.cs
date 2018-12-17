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
    public class LocationController : Controller
    {
        public dbr.LocationRepository locationRepo;

        public LocationController(dbr.LocationRepository repo)
        {
            locationRepo = repo;
        }
        
        // GET: Location
        public ActionResult Index()
        {
            Sync.LocationProcessor.Initialize(locationRepo.Database);
            Sync.LocationProcessor.SyncFromDatabase();

            return View(lib.Location.Locations);
        }

        // GET: Location/Details/5
        public ActionResult Details(int id)
        {
            return View(lib.Location.GetById(id));
        }

        // GET: Location/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Location/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(lib.Location formLocation)
        {
            try
            {

                db.Location dbLocation = new db.Location();
                List<db.InventoryJunction> dbInventory;
                dbLocation = db.Mapper.Map(formLocation, out dbInventory);

                locationRepo.Create(dbLocation);
                foreach (var ingredient in dbInventory)
                {
                    locationRepo.inventoryRepo.Create(ingredient);
                }

                locationRepo.SaveChanges();

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: Location/Edit/5
        public ActionResult Edit(int id)
        {
            return View(lib.Location.GetById(id));
        }

        // POST: Location/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, lib.Location formLocation)
        {
            try
            {
                lib.Location libLocation = lib.Location.GetById(formLocation.Id);
                Dictionary<int, int> formInventory = formLocation.Inventory;
                lib.Location.Locations.Remove(formLocation);

                foreach (var ingredient in formInventory)
                {
                    libLocation.UpdateInventory(ingredient.Key, ingredient.Value - libLocation.Inventory[ingredient.Key]);
                }

                db.Location dbLocation;
                List<db.InventoryJunction> inventoryList;
                dbLocation = db.Mapper.Map(libLocation, out inventoryList);

                locationRepo.Update(dbLocation);
                foreach(var inventory in inventoryList)
                {
                    locationRepo.inventoryRepo.Update(inventory);
                }
                locationRepo.SaveChanges();

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View(formLocation);
            }
        }

        // GET: Location/Delete/5
        public ActionResult Delete(int id)
        {
            return View(lib.Location.GetById(id));
        }

        // POST: Location/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, lib.Location formLocation)
        {
            try
            {
                lib.Location libLocation = lib.Location.GetById(formLocation.Id);
                lib.Location.Locations.Remove(formLocation);
                db.Location dbLocation;
                List<db.InventoryJunction> dbInventories;
                dbLocation = db.Mapper.Map(libLocation, out dbInventories);

                foreach (var inventory in dbInventories)
                {
                    locationRepo.inventoryRepo.Delete(inventory);
                }
                locationRepo.Delete(dbLocation);
                locationRepo.SaveChanges();
                lib.Location.Locations.Remove(libLocation);

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}