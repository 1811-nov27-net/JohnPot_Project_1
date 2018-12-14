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
            DefaultLocation = new Location();
        }

        public int    Id                { get; set; }
        public string FirstName         { get; set; }
        public string LastName          { get; set; }
        public int?   DefaultLocationId { get; set; }

        // Navigation Property
        public Location DefaultLocation { get; set; }
        public ICollection<Order>    Order           { get; set; }
    }
}
