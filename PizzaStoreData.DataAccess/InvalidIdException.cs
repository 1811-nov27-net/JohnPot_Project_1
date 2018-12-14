using System;
using System.Collections.Generic;
using System.Text;

namespace PizzaStoreData.DataAccess
{
    public class InvalidIdException : Exception
    {
        public InvalidIdException(string message)
            : base(message) { }
    }
}
