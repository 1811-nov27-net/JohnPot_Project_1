using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using e = PizzaStoreLibrary.library.Exceptions;

namespace PizzaStoreLibrary.library.Models
{
    public class Ingredient
    {
        public static List<Ingredient> Ingredients = new List<Ingredient>();

        private static int currentId = 0;
        private static int NextId
        {
            get
            {
                int? newId = Ingredients.OrderByDescending(l => l.id).FirstOrDefault()?.id;

                return newId == null ? 0 : currentId = (int)++newId;
            }
        }

        private static void SanitizeIngredientName(ref string name)
        {
            // Remove plural forms
            if (name.EndsWith("s"))
                name = name.Remove(name.Length - 1);

            name = name.ToLower();
        }

        public static implicit operator Ingredient(string name)
        {
            SanitizeIngredientName(ref name);
            Ingredient ingredient = GetByName(name);
            return ingredient ?? throw new e.IngredientDoesNotExistException($"Ingredient Conversion: Could not find {name}.");
        }

        private int id;
        public int Id
        {
            get
            {
                if (id == 0)
                    throw new e.InvalidIdAccessException($"Location {Name} has not been provided a valid Id.");

                return id;
            }

            set => id = value;
        }

        private string _name;
        public string  Name
        {
            get => _name;
            set
            {
                SanitizeIngredientName(ref value);
                _name = value;
            }

        }
        
        public decimal Price { get; set; }

        public Ingredient()
        {
            //if(Id == 0)
                Id = NextId;
            Ingredients.Add(this);
        }
            
        public static Ingredient GetById(int id)
        {
            Ingredient ingredient = Ingredients.FirstOrDefault(i => i.Id == id);
            return ingredient ?? throw new e.IngredientDoesNotExistException($"IngredientId {id} could not be fouund.");
        }

        public static Ingredient GetByName(string name)
        {
            SanitizeIngredientName(ref name);
            Ingredient ingredient = Ingredients.FirstOrDefault(i => i.Name == name);
            return ingredient ?? throw new e.IngredientDoesNotExistException($"IngredientId {name} could not be fouund.");
        }
    }
}
