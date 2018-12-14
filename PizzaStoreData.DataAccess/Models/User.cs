using System;
using System.Collections.Generic;
using System.Text;

namespace PizzaStoreData.DataAccess.Models
{
    public class User
    {
        public User()
        {
            Order = new HashSet<Order>();
        }

        public int    Id                { get; set; }
        public string FirstName         { get; set; }
        public string LastName          { get; set; }
        public int?   DefaultLocationId { get; set; }

        // Navigation Property
        public ICollection<Order> Order { get; set; }
    }
}
