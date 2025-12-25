using System;

namespace Ashutosh.Common.Logger
{
    internal class FallbackLogger : ILogger
    {
        public void Log(LogData logData)
        {
            string typeWithIds = String.Format(
                "{0} ({1}, {2})",
                logData.ModuleName ?? logData.NameSpace,
                logData.ProcessId,
                logData.ThreadId);

            string fullMsg = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + " " + typeWithIds + ": " + logData.Message + " " + logData.Exception.Replace("»", Environment.NewLine);
            System.Diagnostics.Trace.WriteLine(fullMsg);
            
        }
    }
}
