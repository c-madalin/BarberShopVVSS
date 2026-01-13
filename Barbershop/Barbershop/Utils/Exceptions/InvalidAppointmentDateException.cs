using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;

namespace Barbershop.Utils.Exceptions
{
    public class InvalidAppointmentDateException : Exception
    {
        public InvalidAppointmentDateException(string message) : base(message)
        {
        }

        public InvalidAppointmentDateException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}