using Barbershop.Utils.Logging.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Barbershop.Utils.Logging.Interface
{
    internal interface IAppLogger
    {
        void Log(string message, LogLevel level);
    }
}
