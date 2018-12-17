using System;
using System.Collections.Generic;
using System.Text;

namespace PizzaStoreLibrary.library.Exceptions
{
    public class UninitializedProcessorException : Exception
    {
        public UninitializedProcessorException(string message) : base(message)
        {
        }
    }
}
