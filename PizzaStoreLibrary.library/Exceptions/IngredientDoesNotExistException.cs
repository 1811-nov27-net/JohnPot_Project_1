using System;
using System.Collections.Generic;
using System.Text;

namespace PizzaStoreLibrary.library.Exceptions
{
    public class IngredientDoesNotExistException : Exception
    {
        public IngredientDoesNotExistException(string message) : base(message)
        {
        }
    }
}
