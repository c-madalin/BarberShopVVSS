using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Barbershop.Utils.Exceptions
{
    public class BarberUnavailableException : Exception
    {
        public BarberUnavailableException(string message) : base(message)
        {
        }

        public BarberUnavailableException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}