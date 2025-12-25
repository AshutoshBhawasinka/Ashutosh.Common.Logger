using System;
using System.Collections.Generic;

namespace Ashutosh.Common.Logger
{
    public class ConsoleLogger : ILogger
    {
        public static object consoleColorSyncLock = new object();
        public static ConsoleColor DefaultConsoleForegroundColor = Console.ForegroundColor;

        private static readonly Dictionary<Severity, ConsoleColor> logLevelColor = new Dictionary<Severity, ConsoleColor>
        {
            {Severity.Info, DefaultConsoleForegroundColor },
            {Severity.Verbose, ConsoleColor.DarkGray },
            {Severity.Error, ConsoleColor.Red },
            {Severity.Warning, ConsoleColor.Yellow },
            {Severity.Fatal, ConsoleColor.DarkRed },
            {Severity.None, DefaultConsoleForegroundColor}
        };

        public void Log(LogData logData)
        {
            string typeWithIds = String.Format(
                "{0} ({1}, {2})", 
                logData.ModuleName??logData.NameSpace,
                logData.ProcessId, 
                logData.ThreadId);

            string fullMsg = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + " " + typeWithIds + ": " + logData.Message + " " + logData.Exception.Replace("»", Environment.NewLine);
            //System.Diagnostics.Trace.WriteLine(fullMsg);

            lock (consoleColorSyncLock)
            {
                Console.ForegroundColor = logLevelColor[logData.Severity];
                Console.WriteLine(fullMsg);
                Console.ForegroundColor = DefaultConsoleForegroundColor;
            }
        }
    }
}
