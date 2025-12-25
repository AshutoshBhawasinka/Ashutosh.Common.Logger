using System;
using System.Diagnostics;
using System.Threading;

namespace Ashutosh.Common.Logger
{
    public class LogData
    {
        private static readonly string defaultProcessName = GetProcessName();
        private static readonly int defaultProcessId = Process.GetCurrentProcess().Id;
        private static readonly string defaultMachineName = Environment.MachineName;
        private static readonly string defaultUserName  = Environment.UserDomainName + "\\" + Environment.UserName;

        public string ModuleName { get; set; }
        
        public string Message { get; set; }
        public Severity Severity { get; set; }
        public string NameSpace { get; set; }

        public string Exception { get; set; }

        public DateTime DateTime { get; set; }

        public string ProcessName { get; set; } = defaultProcessName;

        public int ProcessId { get; set; } = defaultProcessId;

        public string ThreadName { get; set; }

        public int ThreadId { get; set; }

        public string MachineName { get; set; } = defaultMachineName;

        public string UserName { get; set; } = defaultUserName;

        public LogData()
        {
            DateTime = DateTime.Now;
            
            ThreadId = Thread.CurrentThread.ManagedThreadId;
            ThreadName = Thread.CurrentThread.Name;

        }

        private static string GetProcessName()
        {
            string name = Environment.GetEnvironmentVariable("ProcessAlias");
            if (string.IsNullOrWhiteSpace(name))
            {
                name = Process.GetCurrentProcess().ProcessName;
            }
            return name;
        }

    }
}