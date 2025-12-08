using System;

namespace Barbershop.Utils.Exceptions
{
    internal class UserAlreadyExistsException : Exception
    {
        public UserAlreadyExistsException(string message) : base(message)
        {
        }

        public UserAlreadyExistsException(string message, Exception innerException) 
            : base(message, innerException)
        {
        }
    }
}
