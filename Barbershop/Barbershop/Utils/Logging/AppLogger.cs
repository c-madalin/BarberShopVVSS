using Barbershop.Utils.Logging.Enum;
using Barbershop.Utils.Logging.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Barbershop.Utils.Logging
{
    internal static class AppLogger
    {
        private static IAppLogger? _instance;
        public static IAppLogger Init(IAppLogger logger) => _instance = logger;
        public static void Info(string message) => _instance?.Log(message, LogLevel.Info);
        public static void Warn(string message) => _instance?.Log(message, LogLevel.Warning);
        public static void Error(string message) => _instance?.Log(message, LogLevel.Error);
    }
}
