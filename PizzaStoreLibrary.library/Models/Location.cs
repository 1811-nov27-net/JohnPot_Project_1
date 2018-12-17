using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using e = PizzaStoreLibrary.library.Exceptions;

namespace PizzaStoreLibrary.library.Models
{
    public class Location
    {
        public static List<Location> Locations;
        static Location()
        {
            Locations = new List<Location>();
        }

        private static int currentId = 0;
        private static int NextId
        {
            get
            {
                int? newId = Locations.OrderByDescending(l => l.id).FirstOrDefault()?.id;

                return newId == null ? 0 : currentId = (int)++newId;
            }
        }

        public Location()
        {
            Id = NextId;
            // Let's give all locations a base
            //  list of ingredients (inventory count will
            //  still default to 0). 
            foreach(var ingredient in Ingredient.Ingredients)
            {
                Inventory.Add(ingredient.Id, 0);
            }
            Locations.Add(this);
        }

        public static Location GetById(int id)
        {
            Location location = Locations.FirstOrDefault(l => l.Id == id);

            return location ?? throw new e.InvalidIdException($"Location GetById could not find {id}.");
        }
        public static Location GetByName(string name)
        {
            Location location = Locations.FirstOrDefault(l => l.Name == name);

            return location ?? throw new e.InvalidNameException($"Location {name} does not exist.");
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

        public string Name { get; set; }
        private Dictionary<int, int> _inventory = new Dictionary<int, int>();
        public  Dictionary<int, int> Inventory { get => _inventory; }

        public void UpdateInventory(string ingredientName, int count)
        {
            Ingredient ingredient = Ingredient.GetByName(ingredientName);
            
            Inventory[ingredient.Id] += count;

            if(Inventory[ingredient.Id] < 0)
            {
                // Reset back to a valid state
                Inventory[ingredient.Id] -= count;
                throw new e.InsufficientIngredientException($"Location {Name} does not contain enough {ingredient} to fulfill depletion request");
            }
        }

        public void UpdateInventory(int ingredientId, int count)
        {
            Ingredient ingredient = Ingredient.GetById(ingredientId);
            UpdateInventory(ingredient.Name, count);
        }
        
    }
}
