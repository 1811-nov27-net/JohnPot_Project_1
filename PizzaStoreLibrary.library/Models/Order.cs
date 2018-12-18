using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using e = PizzaStoreLibrary.library.Exceptions;

namespace PizzaStoreLibrary.library.Models
{
    public class Order
    {
        public static bool OrderHasBeenPlace = false;
        public static List<Order> Orders;
        static Order()
        {
            Orders = new List<Order>();
        }

        public Order()
        {
            Id = NextId;
            Orders.Add(this);
        }

        private static int currentId = 0;
        private static int NextId
        {
            get
            {
                int? newId = Orders.OrderByDescending(l => l.id).FirstOrDefault()?.id;

                return newId == null ? 1 : currentId = (int)++newId;
            }
        }

        public static Order GetById(int id)
        {
            Order order = Orders.FirstOrDefault(o => o.Id == id);

            return order ?? throw new e.InvalidIdException($"Order GetById could not find {id}.");
        }

        private int id;
        public int Id
        {
            get
            {
                return id;
            }

            set => id = value;
        }

        private int locationId;
        public int LocationId
        {
            get
            {
                if (locationId == 0)
                    throw new e.InvalidIdAccessException($"Order LocationId has not been provided a valid Id.");

                return locationId;
            }

            set => locationId = value;
        }
        private int userId;
        public int UserId
        {
            get
            {
                if (userId == 0)
                    throw new e.InvalidIdAccessException($"Order UserId has not been provided a valid Id.");

                return userId;

            }
            set => userId = value;
        }
        public DateTime TimePlaced { get; set; }
        public decimal TotalPrice
        {
            get
            {
                decimal totalPrice = 0.0m;
                foreach(Pizza pizza in Pizzas)
                {
                    totalPrice += pizza.Price;
                }

                return totalPrice;
            }
        }

        public List<Pizza> Pizzas = new List<Pizza>();

        public void AddPizzaToOrder(Pizza pizza)
        {
            Pizzas.Add(pizza);
        }
        public void AddPizzaToOrder(params string[] ingredients)
        {
            Pizza pizza = new Pizza();
            pizza.AddIngredientsToPizza(ingredients);
            AddPizzaToOrder(pizza);
        }
    }
}
