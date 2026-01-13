using System;

namespace Barbershop.Utils.Exceptions
{
    public class InvalidSalaryException : Exception
    {
        public InvalidSalaryException(string message) : base(message)
        {
        }

        public InvalidSalaryException(string message, Exception innerException) 
            : base(message, innerException)
        {
        }
    }
}
