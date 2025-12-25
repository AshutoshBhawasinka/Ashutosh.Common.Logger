#region (C) Ashutosh Bhawasinka
// 
// All rights are reserved. Reproduction or transmission in whole or in part,
// in any form or by any means, electronic, mechanical or otherwise, is
// prohibited without the prior written permission of the copyright owner.
// 
// Filename:Logger.cs
// 
// ReSharper disable SuggestUseVarKeywordEvident
// ReSharper disable ConditionIsAlwaysTrueOrFalse
// ReSharper disable RedundantToStringCall
// ReSharper disable CheckNamespace
#endregion
using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text;
using System.Diagnostics;
using System.Data.Common;
using System.Net.Sockets;
using System.Threading;

[module: SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", Scope = "namespace", Target = "Ashutosh.Logger", MessageId = "Ashutosh")]
[module: SuppressMessage("Microsoft.Design", "CA1020:AvoidNamespacesWithFewTypes", Scope = "namespace", Target = "Ashutosh.Logger")]
namespace Ashutosh.Common.Logger
{
    /// <summary>
    /// Class to facilitate custom/automatic logging.
    /// </summary>
    public sealed class Log : IDisposable
    {
        Logger _logger;
        private static readonly CultureInfo culture = CultureInfo.CurrentCulture;
        /// <summary>
        /// Writes an log entry to the .net tracer or the standard windows debug output stream
        /// </summary>
        /// <param name="message">The message to display/log</param>
        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        public void WriteLine(string message)
        {
            StringBuilder str = new StringBuilder();
            int indentLevel = GetIndentLevel();
            for (int l = 0; l < indentLevel; l++)
                str.Append(" ");

            //Append the timestamp and the thread id before the message.
            message = String.Format(culture, "{0} (0x{1:X}):{2}{3}", DateTime.Now.ToString(), Thread.CurrentThread.ManagedThreadId.ToString(culture), str.ToString(), message);

        }

        /// <summary>
        /// A helper method to write a database error message.
        /// </summary>
        /// <param name="message">Custom message</param>
        /// <param name="exception">A socket exception object.</param>
        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters")]
        public void WriteSocketErrorMessage(string message, SocketException exception)
        {
            if (null != exception)
            {
                message = String.Format(culture, "{0}. Error Code <0x{1:X}> - {2}", message, exception.ErrorCode, exception.Message);
            }
            WriteLine(message);
        }

        /// <summary>
        /// Helper function to write the error or exception message related to database operation
        /// </summary>
        /// <param name="message">User defined custom message</param>
        /// <param name="exception">DbException object</param>
        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters")]
        public void WriteDatabaseErrorMessage(string message, DbException exception)
        {
            if (null != exception)
            {
                message = String.Format(culture, "{0}. Error Code <0x{1:X}> - {2}", message, exception.ErrorCode, exception.Message);
            }
            WriteLine(message);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ex"></param>
        public void LogException(Exception ex)
        {
            if (null == ex) return;
            WriteLine("Type: " + ex.GetType().FullName);
            WriteLine("Message: " + ex.Message);
            WriteLine("StackTrace: " + ex.StackTrace);
            
            while (ex != null)
            {
                WriteLine("Inner Exception Type: " + ex.GetType().FullName);
                WriteLine("Inner Exception Message: " + ex.Message);
                WriteLine("Inner Exception StackTrace: " + ex.StackTrace);
                ex = ex.InnerException;
            }
            Trace.Flush();
        }


        /// <summary>
        /// Flag which indicates if the object was destroyed as a call to it'fileData destructor or Dispose()
        /// </summary>
        private bool _disposed;

        /// <summary>
        /// Method name for which the class is currently facilitating the logging.
        /// </summary>
        private readonly string _methodName = "";

        /// <summary>
        /// Constructor. 
        /// Methods which require automatic logging must create an object of this class using the "using" construct ONLY
        /// </summary>
        /// <param name="loggingSource"></param>
        /// <param name="methodName">The name of the method logging for</param>
        public Log(Type loggingSource, string methodName)
        {
            _logger = new Logger(loggingSource);
            _methodName = methodName;
            WriteLine(String.Format(culture, "> {0}()", methodName));
            IncreaseIndentLevel();
        }

        /// <summary>
        /// Constructor. 
        /// Methods which require automatic logging must create an object of this class using the "using" construct ONLY
        /// </summary>
        /// <param name="methodName">The name of the method logging for</param>
        /// <param name="args">The arguments whose values are to be displayed in the traces</param>
        public Log(Type loggingSource, string methodName, params object[] args)
        {
            _logger = new Logger(loggingSource);
            _methodName = methodName;

            StringBuilder str = new StringBuilder();

            str.Append("> ");
            str.Append(methodName);
            str.Append("(");

            //Add all the arguments
            if (null != args)
            {
                for (int i = 0; i < args.Length; i++)
                {
                    if (null != args[i])
                    {
                        str.Append(args[i]);
                    }
                    else
                    {
                        str.Append("(null)");
                    }
                    if (i != args.Length - 1)
                        str.Append(", ");
                }
            }
            str.Append(")");
            WriteLine(str.ToString());
            IncreaseIndentLevel();
        }
        /// <summary>
        /// 
        /// </summary>
        ~Log()
        {
            Dispose();
        }

        #region IDisposable Members
        /// <summary>
        /// Method which is called when the object is disposed/destroyed. Facilitate automatic tracing/logging
        /// </summary>
        public void Dispose()
        {
            if (!_disposed)
            {
                DecreaseIndentLevel();
                WriteLine(String.Format(culture, "< {0}()", _methodName));
                _disposed = true;
                GC.SuppressFinalize(this);
            }
        }

        #endregion

        /// <summary>
        /// Returns the current indent level for the calling thread.
        /// </summary>
        /// <returns>Indent level for the calling thread</returns>
        static int GetIndentLevel()
        {
            int currentLevel = 0;
            LocalDataStoreSlot slot;
            if (null != (slot = Thread.GetNamedDataSlot("IndentLevel")))
            {
                object objVal = Thread.GetData(slot);
                if (null != objVal && objVal is int)
                    currentLevel = (int)objVal;
            }
            return currentLevel;
        }

        /// <summary>
        /// Increases the indent level for the calling thread and stores it in thread local storage
        /// </summary>
        static void IncreaseIndentLevel()
        {
            LocalDataStoreSlot slot;
            int i = 0;
            if (null != (slot = Thread.GetNamedDataSlot("IndentLevel")))
            {
                object objVal = Thread.GetData(slot);
                if (null != objVal && objVal is int)
                    i = (int)objVal;
                i += 2;
                Thread.SetData(slot, i);
            }
        }

        /// <summary>
        /// Decreases the indent level for the calling thread and stores it in thread local storage
        /// </summary>
        static void DecreaseIndentLevel()
        {
            int i = 0;
            LocalDataStoreSlot slot;

            if (null != (slot = Thread.GetNamedDataSlot("IndentLevel")))
            {
                object objVal = Thread.GetData(slot);
                if (null != objVal && objVal is int)
                    i = (int)objVal;

                if (i >= 2)
                    i -= 2;
                else
                    i = 0;
                Thread.SetData(slot, i);
            }
        }
    }
}

// ReSharper restore RedundantToStringCall
// ReSharper restore ConditionIsAlwaysTrueOrFalse
// ReSharper restore SuggestUseVarKeywordEvident
