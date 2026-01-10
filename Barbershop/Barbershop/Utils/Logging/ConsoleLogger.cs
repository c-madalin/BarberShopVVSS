using Barbershop.Utils.Logging.Enum;
using Barbershop.Utils.Logging.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Barbershop.Utils.Logging
{
    public sealed class ConsoleLogger : IAppLogger
    {
        public void Log(string message, LogLevel level)
        {
            var color = level switch
            {
                LogLevel.Info => ConsoleColor.Green,
                LogLevel.Warning => ConsoleColor.Yellow,
                LogLevel.Error => ConsoleColor.Red,
                _ => ConsoleColor.White
            };
            Console.ForegroundColor = color;
            Console.WriteLine($"[{level}] {message}");
            Console.ResetColor();
        }
    }
}
