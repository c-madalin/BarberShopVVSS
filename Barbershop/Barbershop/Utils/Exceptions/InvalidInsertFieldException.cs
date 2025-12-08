using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Barbershop.Utils.Exceptions
{
    internal class InvalidInsertFieldException: Exception
    {
        public InvalidInsertFieldException(string message) : base(message)
        {
        }

        public InvalidInsertFieldException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
