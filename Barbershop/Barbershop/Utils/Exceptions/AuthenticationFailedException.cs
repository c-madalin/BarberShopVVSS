using System;

namespace Barbershop.Utils.Exceptions
{
    public class AuthenticationFailedException : Exception
    {
        public AuthenticationFailedException(string message) : base(message)
        {
        }

        public AuthenticationFailedException(string message, Exception innerException) 
            : base(message, innerException)
        {
        }
    }
}
