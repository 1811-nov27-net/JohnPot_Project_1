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
                Id = libUser.Id,
                FirstName = libUser.FirstName,
                LastName = libUser.LastName
            };

            try
            {
                if (libUser.DefaultLocationId != null)
                    viewUser.DefaultLocationName = lib.Location.GetById((int)libUser.DefaultLocationId).Name;
                else
                    viewUser.DefaultLocationName = null;
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
            if (viewUser.Id != 0)
                libUser.Id = viewUser.Id;

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

        /***** Library List -> View List *****/
        public static List<view.User> Map(List<lib.User> libUserList)
        {
            List<view.User> viewUserList = new List<view.User>();
            foreach (var libUser in libUserList)
            {
                viewUserList.Add(Map(libUser));
            }
            return viewUserList;
        }

        #endregion

        #region Order Mapping
        /***** Library -> View *****/
        public static view.Order Map(lib.Order libOrder)
        {
            view.Order viewOrder = new view.Order
            {
                Id = libOrder.Id,
                TimePlaced = libOrder.TimePlaced,
                TotalPrice = libOrder.TotalPrice
                
            };

            try
            {
                viewOrder.Location      = lib.Location.GetById(libOrder.LocationId).Name;
                lib.User u = lib.User.GetById(libOrder.UserId);
                viewOrder.UserName = u.FirstName + " " + u.LastName;
            }
            catch (e.InvalidIdException e)
            {
                throw (e);
            }

            return viewOrder;
        }
        /***** Library List -> View List *****/
        public static List<view.Order> Map(List<lib.Order> libOrderList)
        {
            List<view.Order> viewOrderList = new List<view.Order>();

            foreach (var libOrder in libOrderList)
            {
                viewOrderList.Add(Map(libOrder));
            }

            return viewOrderList;
        }
        /***** View -> Library *****/
        public static lib.Order Map(view.Order viewOrder)
        {
            lib.Order libOrder = new lib.Order
            {
                TimePlaced = viewOrder.TimePlaced,

            };
            if (viewOrder.Id != 0)
                libOrder.Id = viewOrder.Id;
            
            try
            {
                libOrder.LocationId = lib.Location.GetByName(viewOrder.Location).Id;
                libOrder.UserId = lib.User.GetByName(viewOrder.UserFirstName, viewOrder.UserLastName).Id;
            }
            catch (e.InvalidNameException e)
            {
                throw (e);
            }

            return libOrder;
        }
        


        #endregion
    }
}
