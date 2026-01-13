using Barbershop.Utils.Logging.Enum;
using Barbershop.Utils.Logging.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Barbershop.Utils.Logging
{
    internal sealed class FileLogger: IAppLogger, IDisposable
    {
        private readonly StreamWriter _writer;
        private LogLevel _minimumLevel;

        public FileLogger(string filename)
        {
            _writer = new StreamWriter(path: filename, append: true);
            _writer.AutoFlush = true;
            _minimumLevel = LogLevel.Info;
        }

        public void Log(string message, LogLevel level)
        {
            if (level < _minimumLevel) return;

            string prefix = level switch
            {
                LogLevel.Info => "[INFO]    ",
                LogLevel.Warning => "[WARNING] ",
                LogLevel.Error => "[ERROR]   ",
                _ => "[UNKNOWN] "
            };

            string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            _writer.WriteLine($"{prefix}[{timestamp}] {message}");
        }

        public void SetMinimumLevel(LogLevel minimumLevel)
        {
            _minimumLevel = minimumLevel;
        }

        public void Dispose() => _writer?.Dispose();
    }
}
