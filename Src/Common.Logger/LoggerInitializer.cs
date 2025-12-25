using System.Collections.Generic;

namespace Ashutosh.Common.Logger
{
    /// <summary>
    /// Provides means to control the logging
    /// </summary>
    public static class LoggerInitializer
    {
        /// <summary>
        /// Gets or sets a value indicating if the logs shall also be written to console irrespective
        /// of the loggers added or discovered
        /// </summary>
        public static bool ForceLogToConsole { get; set; } = true;

        internal static List<ILogger> AdditionalLoggers = new List<ILogger>();

        /// <summary>
        /// Adds additional logger destination to which all log messages should be sent.
        /// </summary>
        /// <param name="logger">The logger to be added</param>
        public static void AddAdditionalLogger(ILogger logger)
        {
            AdditionalLoggers.Add(logger);
        }
    }
}
