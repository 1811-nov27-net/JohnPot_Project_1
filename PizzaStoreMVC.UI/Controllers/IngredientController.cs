using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using db = PizzaStoreData.DataAccess.Models;
using dbr = PizzaStoreData.DataAccess.Repositories;
using lib = PizzaStoreLibrary.library.Models;

namespace PizzaStoreMVC.UI.Controllers
{
    public class IngredientController : Controller
    {
        public dbr.IngredientRepository ingredientRepo;

        public IngredientController(dbr.IngredientRepository repo)
        {
            ingredientRepo = repo;
        }

        // GET: Ingredient
        public ActionResult Index()
        {
            Sync.IngredientProcessor.Initialize(ingredientRepo.Database);
            Sync.IngredientProcessor.SyncFromDatabase();

            return View(lib.Ingredient.Ingredients);
        }
        
        // GET: Ingredient/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Ingredient/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(lib.Ingredient formIngredient)
        {
            try
            {
                lib.Ingredient libIngredient = new lib.Ingredient
                {
                    Name = formIngredient.Name,
                    Price = formIngredient.Price
                };
                lib.Ingredient.Ingredients.Remove(formIngredient);
                ingredientRepo.Create(db.Mapper.Map(libIngredient));
                ingredientRepo.SaveChanges();

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: Ingredient/Edit/5
        public ActionResult Edit(int id)
        {
            return View(lib.Ingredient.GetById(id));
        }

        // POST: Ingredient/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, lib.Ingredient formIngredient)
        {
            try
            {
                lib.Ingredient libIngredient = lib.Ingredient.GetById(formIngredient.Id);
                lib.Ingredient.Ingredients.Remove(formIngredient);
                libIngredient.Price = formIngredient.Price;

                ingredientRepo.Update(db.Mapper.Map(libIngredient));
                ingredientRepo.SaveChanges();

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: Ingredient/Delete/5
        public ActionResult Delete(int id)
        {
            return View(lib.Ingredient.GetById(id));
        }

        // POST: Ingredient/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, lib.Ingredient formIngredient)
        {
            try
            {
                lib.Ingredient libIngredient = lib.Ingredient.GetById(formIngredient.Id);
                lib.Ingredient.Ingredients.Remove(formIngredient);
                lib.Ingredient.Ingredients.Remove(libIngredient);
                ingredientRepo.Delete(db.Mapper.Map(libIngredient));
                ingredientRepo.SaveChanges();

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}