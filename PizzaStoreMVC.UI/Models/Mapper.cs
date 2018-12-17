using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using view = PizzaStoreMVC.UI.Models;
using lib = PizzaStoreLibrary.library.Models;
using e = PizzaStoreLibrary.library.Exceptions;

namespace PizzaStoreMVC.UI.Models
{
    public static class Mapper
    {
        #region User Mapping
        /***** Library -> View *****/
        public static view.User Map(lib.User libUser)
        {
            view.User viewUser = new view.User
            {
                FirstName = libUser.FirstName,
                LastName = libUser.LastName
            };

            try
            {
                viewUser.DefaultLocationName = lib.Location.GetById((int)libUser.DefaultLocationId).Name;
            }
            catch(e.InvalidIdException)
            {
                viewUser.DefaultLocationName = null;
            }

            return viewUser;
        }

        /***** View -> Library *****/
        public static lib.User Map(view.User viewUser)
        {
            lib.User libUser = new lib.User
            {
                FirstName = viewUser.FirstName,
                LastName = viewUser.LastName
            };

            try
            {
                libUser.DefaultLocationId = lib.Location.GetByName(viewUser.DefaultLocationName).Id;
            }
            catch (e.InvalidNameException)
            {
                viewUser.DefaultLocationName = null;
            }

            return libUser;
        }

        #endregion  
    }
}
