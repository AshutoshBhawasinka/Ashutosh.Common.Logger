
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;

namespace Ashutosh.Common.Logger
{
    /// <summary>
    /// Provides logging methods 
    /// </summary>
    public partial class Logger
    {
        private readonly string _moduleName;
        private readonly string _nameSpace;

        private static readonly List<ILogger> Loggers = InitializeLoggers();

        private static List<ILogger> InitializeLoggers()
        {
            List<ILogger> allLoggers = new List<ILogger>();
            if (LoggerInitializer.ForceLogToConsole)
            {
                allLoggers.Add(new ConsoleLogger());
            }

            var lgr = LoggerContainer.GetLogger();
            if (lgr != null && lgr.GetType() != typeof(ConsoleLogger))
            {
                allLoggers.Add(lgr);
            }

            if (LoggerInitializer.AdditionalLoggers.Count > 0)
            {
                foreach (ILogger logger in LoggerInitializer.AdditionalLoggers)
                {
                    allLoggers.Add(logger);
                }
            }
            if (lgr == null && LoggerInitializer.AdditionalLoggers.Count == 0 && !LoggerInitializer.ForceLogToConsole)
            {
                allLoggers.Add(new FallbackLogger());
            }
            AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;
            return allLoggers;
        }
        /// <summary>
        /// Creates an instance of the logger for a module identified by 
        /// </summary>
        public Logger(Type type)
        {
            _moduleName = type.Name;
            _nameSpace = type.Namespace;
        }
        /// <summary>
        /// Creates an instance of the logger for a module identified by 
        /// <paramref name="moduleName"/>
        /// </summary>
        /// <param name="moduleName">Name of the module to use for generating log messages</param>
        /// <param name="nameSpace">The name space to include in the log message</param>
        public Logger(string moduleName, string nameSpace)
        {
            this._moduleName = moduleName;
            this._nameSpace = nameSpace;
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="args"></param>
        public void LogVerbose(string message, params object[] args)
        {
            Log(Severity.Verbose, message, null, args);
        }
        /// <summary>
        /// Log message with error exception details.
        /// </summary>
        /// <param name="exception"></param>
        /// <param name="message"></param>
        /// <param name="args">message arguments for string.Format</param>
        public void LogVerbose(Exception exception, string message, params object[] args)
        {
            Log(Severity.Verbose, message, exception, args);
        }

        #region Log Methods

        /// <summary>
        /// Log Message
        /// </summary>
        /// <param name="message"></param>
        public void Log(string message)
        {
            Log(Severity.Info, message, null);
        }

        /// <summary>
        /// Log Message with Exception details
        /// </summary>
        /// <param name="message"></param>
        /// <param name="exception"></param>
        public void Log(string message, Exception exception)
        {
            Log(Severity.Info, message,exception);
        }

        /// <summary>
        /// Log message
        /// </summary>
        /// <param name="message"></param>
        /// <param name="args">message arguments for string.Format</param>
        public void Log(string message, params object[] args)
        {
            Log(Severity.Info, message, null, args);
        }

        /// <summary>
        /// Log message with error exception details.
        /// </summary>
        /// <param name="exception"></param>
        /// <param name="message"></param>
        /// <param name="args">message arguments for string.Format</param>
        public void Log(Exception exception, string message, params object[] args)
        {
            Log(Severity.Info, message, exception, args);
        }
        /// <summary>
        /// Log message
        /// </summary>
        /// <param name="message"></param>
        /// <param name="args">message arguments for string.Format</param>
        public void LogError(string message, params object[] args)
        {
            Log(Severity.Error, message,null, args);
        }
        /// <summary>
        /// Log message with error exception details.
        /// </summary>
        /// <param name="exception"></param>
        /// <param name="message"></param>
        /// <param name="args">message arguments for string.Format</param>
        public void LogError(Exception exception, string message, params object[] args)
        {
            Log(Severity.Error, message, exception, args);
        }
        /// <summary>
        /// Log message with error exception details.
        /// </summary>
        /// <param name="exception"></param>
        /// <param name="message"></param>
        /// <param name="args">message arguments for string.Format</param>
        public void LogWarning(Exception exception, string message, params object[] args)
        {
            Log(Severity.Warning, message, exception, args);
        }
        /// <summary>
        /// Log message with error exception details.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="args">message arguments for string.Format</param>
        public void LogWarning(string message, params object[] args)
        {
            Log(Severity.Warning, message, null, args);
        }
        #endregion

        /// <summary>
        /// Writes a log
        /// </summary>
        /// <param name="severity"></param>
        /// <param name="msg">The message to be written</param>
        /// <param name="ex"></param>
        /// <param name="args">Format parameters</param>
        private void Log(Severity severity, string msg, Exception ex, params object[] args)
        {
            string expandedMessage = msg ?? "";
            try
            {
                if (msg != null && args != null && args.Length > 0)
                {
                    expandedMessage = string.Format(CultureInfo.CurrentCulture, msg, args);
                }
            }
            catch (FormatException ex1)
            {
                string err1 = "Error in logger component: " +  ex1.Message + Environment.NewLine + ex1.StackTrace;

                Console.WriteLine(err1);
                LogInternal("Logger", err1, Severity.Error, typeof(Logger).Namespace, ex1);
                expandedMessage = msg;

                if (args != null)
                {
                    foreach (object arg in args)
                    {
                        string argString = null;
                        try
                        {
                            argString = arg.ToString();
                        }
                        catch (Exception ex2)
                        {
                            var err2 = "Failed get string representation of type: " + arg.GetType().FullName;
                            Console.WriteLine(err2);
                            LogInternal("Logger", err2, Severity.Error, typeof(Logger).Namespace, ex2);
                        }
                        expandedMessage += " [Argument: " + (argString ?? "(null)") + "]";
                    }

                }
            }
            LogInternal(_moduleName, expandedMessage, severity, _nameSpace, ex);
        }

        private static void LogInternal(
            string moduleName,
            string message,
            Severity severity,
            string nameSpace,
            Exception ex = null
        ) {

            LogData ld = new LogData();
            ld.ModuleName = moduleName;
            ld.Message = message;
            ld.Severity = severity;
            ld.NameSpace = nameSpace;
            ld.Exception = GetExceptionDetails(ex);

            foreach (ILogger logger in Loggers)
            {
                logger.Log(ld);
            }
        }

        static string GetExceptionDetails(Exception ex)
        {
            string msg = "";

            while (ex != null)
            {
                msg += '[' + ex.GetType().FullName + "] " + ex.Message + Environment.NewLine + ex.StackTrace +
                       Environment.NewLine;
                Exception previousException = ex;
                ex = ex.InnerException;
                if (ex != null)
                {
                    msg += "Inner Exception:" + Environment.NewLine;
                }
                if(previousException.GetType() == typeof(AggregateException))
                {
                    msg += ExpandAggregateException((AggregateException)previousException);
                    break;
                }
            }
            return msg;
        }

        static string ExpandAggregateException(AggregateException exception)
        {
            string msg = "";
            Exception ex;
            foreach(var e in exception.InnerExceptions)
            {
                ex = e;
                msg += GetExceptionDetails(ex);
                ex = ex.InnerException;
                if (ex != null)
                {
                    msg += "Inner Exception:" + Environment.NewLine;
                }
            }
            return msg;
        }
        private static void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Log(nameof(Logger), "An unhandled exception occured.", Severity.Error, (Exception)e.ExceptionObject);
            Thread.Sleep(100);
        }
    }
}
