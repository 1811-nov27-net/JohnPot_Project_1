using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using lib = PizzaStoreLibrary.library.Models;
using e = PizzaStoreLibrary.library.Exceptions;
using dbr = PizzaStoreData.DataAccess.Repositories;
using db = PizzaStoreData.DataAccess.Models;
using view = PizzaStoreMVC.UI.Models;

namespace PizzaStoreMVC.UI.Controllers
{
    public class UserController : Controller
    {
        public dbr.UserRepository userRepo;

        public UserController(dbr.UserRepository repo)
        {
            userRepo = repo;

            Sync.LocationProcessor.Initialize(userRepo.Database);
            Sync.LocationProcessor.SyncFromDatabase();

        }

        // GET: User
        public ActionResult Index()
        {
            return View(lib.User.Users);
        }

        // GET: User/Details/5
        public ActionResult Details(int id)
        {
            return View(lib.User.GetById(id));
        }

        // GET: User/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: User/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(view.User formUser)
        {
            lib.User libUser = view.Mapper.Map(formUser);
            userRepo.Create(db.Mapper.Map(libUser));
            userRepo.SaveChanges();

            return RedirectToAction(nameof(Index));
        }

        // GET: User/Edit/5
        public ActionResult Edit(int id)
        {
            return View(lib.User.GetById(id));
        }

        // POST: User/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, view.User formUser)
        {
            try
            {
                lib.User libUser = lib.User.GetById(id);
                libUser.FirstName = formUser.FirstName;
                libUser.LastName = formUser.LastName;
                try
                {
                    libUser.DefaultLocationId = lib.Location.GetByName(formUser.DefaultLocationName).Id;
                }
                catch(e.InvalidNameException)
                {
                    libUser.DefaultLocationId = null;
                }

                db.User dbUser = db.Mapper.Map(libUser);
                userRepo.Update(dbUser);
                userRepo.SaveChanges();

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: User/Delete/5
        public ActionResult Delete(int id)
        {
            return View(lib.User.GetById(id));
        }

        // POST: User/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, lib.User formUser)
        {
            try
            {
                lib.User libUser = lib.User.GetById(formUser.Id);
                lib.User.Users.Remove(formUser);
                lib.User.Users.Remove(libUser);

                db.User dbUser = db.Mapper.Map(libUser);
                userRepo.Delete(dbUser);
                userRepo.SaveChanges();

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}