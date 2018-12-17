using System;
using System.Collections.Generic;
using System.Text;

namespace PizzaStoreLibrary.library.Exceptions
{
    public class InvalidIdAccessException : Exception
    {
        public InvalidIdAccessException(string message) : base(message)
        {
        }
    }
}
