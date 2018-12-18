using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using lib = PizzaStoreLibrary.library.Models;

namespace PizzaStoreMVC.UI.Models
{
    public class Order
    {
        public int    Id            { get; set; }
        public string Location      { get; set; }
        public DateTime TimePlaced  { get; set; }
        public string UserName { get; set; }
        public string UserFirstName
        {
            get
            {
                return UserName.Substring(0, UserName.IndexOf(' '));
            }
        }
        public string UserLastName
        {
            get
            {
                return UserName.Substring(UserName.IndexOf(' '), UserName.Length - UserName.IndexOf(' ')).Trim();
            }
        }

        private Dictionary<string, int> toppings = new Dictionary<string, int>();
        public Dictionary<string, int> PizzaToppings { get => toppings; }
        public lib.Pizza pizza
        {
            get
            {
                lib.Pizza p = new lib.Pizza();
                foreach(var ingredient in PizzaToppings)
                {
                    for(int i = 0; i < ingredient.Value; ++i)
                    {
                        p.AddIngredientsToPizza(ingredient.Key);
                    }
                }
                return p;
            }
        }
        public Order()
        {
            foreach(var ingredient in lib.Ingredient.Ingredients)
            {
                PizzaToppings.Add(ingredient.Name, 0);
            }
        }
        
        public SelectList userNames
        {
            get
            {
                List<string> users = lib.User.Users.Select(u => new string(u.FirstName + " " +  u.LastName )).ToList();
                return new SelectList(users);
            }
        }
        public SelectList ingredientList
        {
            get
            {
                List<string> ingredients = lib.Ingredient.Ingredients.Select(i => new string(i.Name)).ToList();
                return new SelectList(ingredients);
            }
        }
    }
}
