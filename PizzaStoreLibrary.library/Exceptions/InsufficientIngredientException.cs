using System;
using System.Collections.Generic;
using System.Text;

namespace PizzaStoreLibrary.library.Exceptions
{
    public class InsufficientIngredientException : Exception
    {
        public InsufficientIngredientException(string message) : base(message)
        {
        }
    }
}
