
using System;

namespace Ashutosh.Common.Logger
{
    /// <summary>
    /// Provides logging methods 
    /// </summary>
    public partial class Logger
    {
        public static string ProcessName { get; set; }

        /// <summary>
        /// Logs an message for the module identified by 
        /// <paramref name="moduleName"/> with the passed Severity
        /// </summary>
        /// <param name="moduleName">Name of the module which is generating the log message</param>
        /// <param name="message">Log message</param>
        /// <param name="severity">The severity of the log message</param>
        public static void Log(string moduleName, string message, Severity severity)
        {
            LogInternal(moduleName, message, severity, null);
        }

        /// <summary>
        /// Logs an  message for the module identified by 
        /// <paramref name="moduleName"/> with the passed Severity 
        /// </summary>
        /// <param name="moduleName">Name of the module which is generating the log message</param>
        /// <param name="message">Log message</param>
        /// /// <param name="severity">The severity of the log message</param>
        /// <param name="exception">The exception object to be logged</param>
        public static void Log(
            string moduleName, 
            string message, 
            Severity severity, 
            Exception exception
        ) {
            LogInternal(
                moduleName, 
                message, 
                severity, null, exception);
        }
    }
}
