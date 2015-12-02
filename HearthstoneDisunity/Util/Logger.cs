using System;
using System.IO;

namespace HearthstoneDisunity.Util
{
    internal class Logging
    {
        private static Logging _instance;
        private static readonly string _logFileName = "hs-disunity.log";
        private static readonly string _defaultLocation = ".\\";

        private LogLevel _level;
        private string _logFile;

        public static Logging Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new Logging();
                return _instance;
            }
        }

        private Logging()
        {
            _level = LogLevel.ERROR;
            _logFile = _defaultLocation + _logFileName;
        }

        internal void SetLocation(string path)
        {
            if (Directory.Exists(path))
            {
                _logFile = Path.Combine(path, _logFileName);
            }
            else
            {
                _logFile = Path.Combine(_defaultLocation, _logFileName);
                Log(LogLevel.WARN, "Cannot set location, directory does not exist (" + path + ")");
            }
        }

        internal void SetLogLevel(LogLevel level)
        {
            _level = level;
        }

        internal void Log(LogLevel level, string message)
        {
            try
            {
                using (var writer = new StreamWriter(_logFile))
                {
                    if (level >= _level)
                        writer.WriteLine(message);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Failed to write to log file (" + _logFile + ")");
                Console.WriteLine(e.Message);
            }
        }

        internal void Log(LogLevel level, string format, params string[] strings)
        {
            Log(level, string.Format(format, strings));
        }
    }

    public static class Logger
    {
        private static readonly LogLevel _level = LogLevel.INFO;

        public static void Log(string message)
        {
            Logging.Instance.Log(_level, message);
        }

        public static void Log(string format, params string[] strings)
        {
            Logging.Instance.Log(_level, format, strings);
        }

        public static void Log(LogLevel level, string message)
        {
            Logging.Instance.Log(level, message);
        }

        public static void Log(LogLevel level, string format, params string[] strings)
        {
            Logging.Instance.Log(level, format, strings);
        }

        public static void SetLogLocation(string path)
        {
            Logging.Instance.SetLocation(path);
        }

        public static void SetLogLevel(LogLevel level)
        {
            Logging.Instance.SetLogLevel(level);
        }
    }

    public enum LogLevel
    {
        DEBUG,
        INFO,
        WARN,
        ERROR
    }
}