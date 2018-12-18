using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using e = PizzaStoreLibrary.library.Exceptions;

namespace PizzaStoreLibrary.library.Models
{
    public class Pizza
    {
        public readonly List<Ingredient> Ingredients = new List<Ingredient>();
        public static decimal BasePizzaPrice = 10.0m;

        public static List<Pizza> Pizzas;
        static Pizza()
        {
            Pizzas = new List<Pizza>();
        }

        public Pizza()
        {
            Id = NextId;
            Pizzas.Add(this);
        }

        private static int currentId = 1;
        private static int NextId
        {
            get
            {
                int? newId = Pizzas.OrderByDescending(l => l.id).FirstOrDefault()?.id;

                return newId == null ? 0 : currentId = (int)++newId;
            }
        }

        public static Pizza GetById(int Id)
        {
            Pizza pizza = Pizzas.FirstOrDefault(o => o.Id == Id);

            return pizza ?? throw new e.InvalidIdException($"Pizza GetById could not find {Id}.");
        }

        private int id;
        public int Id
        {
            get
            {
                if (id == 0)
                    throw new e.InvalidIdAccessException($"Pizza {Id} has not been provided a valid Id.");

                return id;
            }

            set => id = value;
        }

        public decimal Price
        {
            get
            {
                decimal totalPrice = 10.0m;
                foreach(Ingredient ingredient in Ingredients)
                    totalPrice += ingredient.Price;

                return totalPrice;
            }
        }

        public void AddIngredientsToPizza(params string[] ingredientList)
        {
            foreach (string name in ingredientList)
            {
                Ingredient ingredient = Ingredient.GetByName(name);
                Ingredients.Add(ingredient ?? 
                    throw new e.IngredientDoesNotExistException($"Ingredient {name} could not be found."));
            }
        }
    }
}
