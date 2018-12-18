using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using e = PizzaStoreLibrary.library.Exceptions;

namespace PizzaStoreLibrary.library.Models
{
    public class User
    {
        public static List<User> Users;


        static User()
        {
            Users = new List<User>();
        }
        public User()
        {
            Id = NextId;
            Users.Add(this);
        }

        public static User GetById(int id)
        {
            User user = Users.FirstOrDefault(u => u.Id == id);
            return user ?? throw new e.InvalidIdException($"User GetById could not find {id}");
        }
        public static User GetByName(string firstName, string lastName)
        {
            User user = Users.FirstOrDefault(u => u.FirstName == firstName && u.LastName == lastName);
            return user ?? throw new e.InvalidNameException($"User GetByName could not find {firstName} {lastName}");
        }

        private static int currentId = 0;
        private static int NextId
        {
            get
            {
                int? newId = Users.OrderByDescending(l => l.id).FirstOrDefault()?.id;

                return newId == null ? 1 : currentId = (int)++newId;
            }
        }

        private int id = 0;
        public int Id
        {
            get
            {
                // If you try to access the Id component
                //  of the lib entity before it has been
                //  added to the database and given
                //  a valid Id, throw an exception. 
                //  You can't use the id if its invalid
                // 0 = invalid id (default int)
                //if (id == 0)
                    //throw new e.InvalidIdAccessException($"User {FirstName} {LastName} has not been provided a valid Id.");

                return id;
            }

            set => id = value;
        }

        private int? defaultLocationId;
        public int? DefaultLocationId
        {
            get
            {
                return defaultLocationId;
            }

            set
            {
                // GetById will throw an exception if id of
                //  location does not exist.
                if(value != null)
                    Location.GetById((int)value);

                defaultLocationId = value;
            }
        }


        public string FirstName { get; set; }
        public string LastName { get; set; }

    }
}
