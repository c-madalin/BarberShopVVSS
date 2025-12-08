using System;

namespace Barbershop.Utils.Exceptions
{
    internal class UserNotFoundException : Exception
    {
        public UserNotFoundException(string message) : base(message)
        {
        }

        public UserNotFoundException(string message, Exception innerException) 
            : base(message, innerException)
        {
        }
    }
}
