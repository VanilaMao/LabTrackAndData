using System;
using System.Diagnostics;

namespace Lab.Common.Logging
{
    public class LogService : ILogService
    {
        private readonly ILogFactory _logFactory;

        public bool IsDebugEnabled
        {
            get { return true; }
        }

        public bool IsErrorEnabled
        {
            get { return true; }
        }

        public bool IsFatalEnabled
        {
            get { return true; }
        }

        public bool IsInfoEnabled
        {
            get { return true; }
        }

        public bool IsWarnEnabled
        {
            get { return true; }
        }

        public bool IsTraceEnabled
        {
            get { return true; }
        }

        public LogService(ILogFactory logFactory)
        {
           _logFactory = logFactory;
        } 

        public void Debug(Exception exception)
        {
            GetLoggerInstance().Debug(exception.Message, exception);
        }

        public void Debug(string format, params object[] args)
        {
            GetLoggerInstance().Debug(string.Format(format, args));
        }

        public void Debug(Exception exception, string format, params object[] args)
        {
            GetLoggerInstance().Debug(string.Format(format, args), exception);
        }

        public void Error(Exception exception)
        {
            GetLoggerInstance().Error(exception.Message, exception);
        }

        public void Error(string format, params object[] args)
        {
            GetLoggerInstance().Error(string.Format(format, args));
        }

        public void Error(Exception exception, string format, params object[] args)
        {
            GetLoggerInstance().Error(string.Format(format, args), exception);
        }

        public void Fatal(Exception exception)
        {
            GetLoggerInstance().Fatal(exception.Message, exception);
        }

        public void Fatal(string format, params object[] args)
        {
            GetLoggerInstance().Fatal(string.Format(format, args));
        }

        public void Fatal(Exception exception, string format, params object[] args)
        {
            GetLoggerInstance().Fatal(string.Format(format, args), exception);
        }

        public void Info(Exception exception)
        {
            GetLoggerInstance().Info(exception.Message, exception);
        }

        public void Info(string format, params object[] args)
        {
            GetLoggerInstance().Info(string.Format(format, args));
        }

        public void Info(Exception exception, string format, params object[] args)
        {
            GetLoggerInstance().Info(string.Format(format, args), exception);
        }

        public void Warn(Exception exception)
        {
            GetLoggerInstance().Warn(exception.Message, exception);
        }

        public void Warn(string format, params object[] args)
        {
            GetLoggerInstance().Warn(string.Format(format, args));
        }

        public void Warn(Exception exception, string format, params object[] args)
        {
            GetLoggerInstance().Warn(string.Format(format, args), exception);
        }

        public void Trace(Exception exception)
        {
            GetLoggerInstance().Trace(exception.Message, exception);
        }

        public void Trace(string format, params object[] args)
        {
            GetLoggerInstance().Trace(string.Format(format, args));
        }

        public void Trace(Exception exception, string format, params object[] args)
        {
            GetLoggerInstance().Trace(string.Format(format, args), exception);
        }

        private ILogger GetLoggerInstance()
        {
            return _logFactory.GetLogger(GetPreviousCallingType());
        }

        private static Type GetPreviousCallingType()
        {
            StackFrame[] frames = new StackTrace().GetFrames();
            if (frames == null)
                return typeof(LogService);
            foreach (StackFrame stackFrame in frames)
            {
                Type declaringType = stackFrame.GetMethod().DeclaringType;
                if (declaringType != typeof(LogService) && declaringType != null)
                    return declaringType;
            }

            return typeof(LogService);
        }
    }
}
