using System;
using System.Collections.Generic;
using System.Text;

namespace PizzaStoreLibrary.library.Exceptions
{
    public class InvalidIdException : Exception
    {
        public InvalidIdException(string message)
            : base(message) { }
    }
}
