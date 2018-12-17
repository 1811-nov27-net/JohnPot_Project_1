using System;
using System.Collections.Generic;
using System.Text;

namespace PizzaStoreLibrary.library.Exceptions
{
    public class InvalidNameException : Exception
    {
        public InvalidNameException(string message) : base(message)
        {
        }
    }
}
