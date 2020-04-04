using System;
using log4net;
using log4net.Core;

namespace Lab.Common.Logging
{
    public class Log4Logger : ILogger
    {
        private readonly ILog _log;
        private readonly Type _type;
        public Log4Logger(ILog log, Type type)
        {
            _log = log;
            _type = type;
        }
        public void Debug(object message)
        {
            _log.Debug(message);
        }

        public void Debug(object message, Exception exception)
        {
            _log.Debug(message, exception);
        }

        public void Error(object message)
        {
            _log.Error(message);
        }

        public void Error(object message, Exception exception)
        {
            _log.Error(message, exception);
        }

        public void Fatal(object message)
        {
            _log.Fatal(message);
        }

        public void Fatal(object message, Exception exception)
        {
            _log.Fatal(message, exception);
        }

        public void Info(object message)
        {
            _log.Info(message);
        }

        public void Info(object message, Exception exception)
        {
            _log.Info(message, exception);
        }

        public void Warn(object message)
        {
            _log.Warn(message);
        }

        public void Warn(object message, Exception exception)
        {
            _log.Warn(message, exception);
        }

        public void Trace(object message)
        {
            _log.Logger.Log(_type, Level.Trace, message, null);
        }

        public void Trace(object message, Exception exception)
        {
            _log.Logger.Log(_type, Level.Trace, message, exception);
        }
    }
}
