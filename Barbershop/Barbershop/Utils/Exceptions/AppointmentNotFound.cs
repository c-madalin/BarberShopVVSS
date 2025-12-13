using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Barbershop.Utils.Exceptions
{
    public class AppointmentNotFoundException : Exception
    {
        public AppointmentNotFoundException(string message) : base(message)
        {
        }

        public AppointmentNotFoundException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}