using System;
using System.IO;

namespace HearthstoneDisunity.Util
{
    public static class Logger
    {
        private static readonly LogLevel _level = LogLevel.INFO;

        public static void Log(string message)
        {
            Logging.Instance.Log(_level, message);
        }

        public static void Log(object obj)
        {
            Logging.Instance.Log(_level, obj);
        }

        public static void Log(string format, params object[] objects)
        {
            Logging.Instance.Log(_level, format, objects);
        }

        public static void Log(LogLevel level, string message)
        {
            Logging.Instance.Log(level, message);
        }

        public static void Log(LogLevel level, object obj)
        {
            Logging.Instance.Log(level, obj);
        }

        public static void Log(LogLevel level, string format, params object[] objects)
        {
            Logging.Instance.Log(level, format, objects);
        }

        public static void SetLogLocation(string path)
        {
            Logging.Instance.Location = path;
        }

        public static void SetLogLevel(LogLevel level)
        {
            Logging.Instance.LogLevel = level;
        }
    }

    public enum LogLevel
    {
        DEBUG,
        INFO,
        WARN,
        ERROR
    }

    internal class Logging
    {
        private static Logging _instance;
        private static readonly string _logFileName = "hs-disunity.log";
        private static readonly string _defaultLocation = ".\\";

        private LogLevel _level;
        private string _logFile;

        private Logging()
        {
            _level = LogLevel.INFO;
            _logFile = _defaultLocation + _logFileName;
        }

        public static Logging Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new Logging();
                return _instance;
            }
        }

        internal LogLevel LogLevel
        {
            get { return _level; }
            set { _level = value; }
        }

        internal string Location
        {
            get { return _logFile; }
            set
            {
                if (Directory.Exists(value))
                {
                    _logFile = Path.Combine(value, _logFileName);
                }
                else
                {
                    _logFile = Path.Combine(_defaultLocation, _logFileName);
                    Log(LogLevel.WARN, "Cannot set location, directory does not exist (" + value + ")");
                }
            }
        }

        internal void Log(LogLevel level, string message)
        {
            if (level >= _level)
            {
                try
                {
                    using (var writer = new StreamWriter(_logFile, true))
                    {
                        var ts = DateTime.Now.ToString("HH:mm:ss");
                        writer.WriteLine("[" + ts + "] " + message);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("Failed to write to log file (" + _logFile + ")");
                    Console.WriteLine(e.Message);
                }
            }
        }

        internal void Log(LogLevel level, string format, params object[] objects)
        {
            Log(level, string.Format(format, objects));
        }

        internal void Log(LogLevel level, object obj)
        {
            Log(level, obj.ToString());
        }
    }
}