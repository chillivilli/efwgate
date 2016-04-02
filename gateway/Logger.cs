using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace EfawateerGateway
{
    static class Logger
    {

        private static bool _detailed;
        private static Action<string> _logAction;

        public static void Initialize(bool detailed, Action<string> logAction)
        {
            _detailed = detailed;
            _logAction = logAction;
        }

        public static void Trace(string message)
        {
            Write(EventType.Trace, message);
        }

        public static void Trace(string format, params object[] args)
        {
            Write(EventType.Trace, format, args);
        }

        public static void Info(string message)
        {
            Write(EventType.Info, message);
        }

        public static void Info(string format, params object[] args)
        {
            Write(EventType.Info, format, args);
        }

        public static void Warn(string message)
        {
            Write(EventType.Warning, message);
        }

        public static void Warn(string format, params object[] args)
        {
            Write(EventType.Warning, format, args);
        }

        public static void Error(string message)
        {
            Write(EventType.Error, message);
        }

        public static void Error(string format, params object[] args)
        {
            Write(EventType.Error, format, args);
        }


        public static void Error(Exception ex)
        {
            if (_detailed)
                Error(ex.ToString());
            else
            {
                Error(ex.Message);
            }
        }

        public static void Error(Exception ex, string message)
        {
            string logMessage = null;
            if (_detailed)
            {
                logMessage = string.Format("{0} : {1}", message, ex);
            }
            else
            {
                logMessage = string.Format("{0} : {1}", message, ex.Message);
            }

            Error(logMessage);
        }


        public static void Critical(string message)
        {
            Write(EventType.CriticalError, message);
        }

        public static void Critical(string format, params object[] args)
        {
            Write(EventType.CriticalError, format, args);
        }


        public static void Critical(Exception ex)
        {
            if (_detailed)
                Critical(ex.ToString());
            else
            {
                Critical(ex.Message);
            }
        }

        public static void Critical(Exception ex, string message)
        {
            string logMessage = null;
            if (_detailed)
            {
                logMessage = string.Format("{0} : {1}", message, ex);
            }
            else
            {
                logMessage = string.Format("{0} : {1}", message, ex.Message);
            }

            Critical(logMessage);
        }


        public static void Write(EventType type, string message)
        {
            if (!_detailed && type == EventType.Trace)
                return;

            string logMessage = string.Format("\t{0}\tThread=[{1}]\t{2}", type, Thread.CurrentThread.ManagedThreadId, message);
            _logAction.Invoke(logMessage);
        }

        public static void Write(EventType type, string format, params object[] args)
        {
            if (!_detailed && type == EventType.Trace)
                return;

            string message = string.Format(format, args);
            Write(type, message);
        }

    }
}
