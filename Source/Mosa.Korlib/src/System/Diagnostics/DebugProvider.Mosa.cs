// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Diagnostics
{
    public partial class DebugProvider
    {
        private static readonly bool s_shouldWriteToStdErr = Environment.GetEnvironmentVariable("COMPlus_DebugWriteToStdErr") == "1";

        public static void FailCore(string stackTrace, string? message, string? detailMessage, string errorSource)
        {
            if (s_FailCore != null)
            {
                s_FailCore(stackTrace, message, detailMessage, errorSource);
                return;
            }

            if (Debugger.IsAttached)
            {
                Debugger.Break();
            }
            else
            {
                // In Core, we do not show a dialog.
                // Fail in order to avoid anyone catching an exception and masking
                // an assert failure.
                DebugAssertException ex = new DebugAssertException(message, detailMessage, stackTrace);
                Environment.FailFast(ex.Message, ex, errorSource);
            }
        }

        public static void WriteCore(string message)
        {
            if (s_WriteCore != null)
            {
                s_WriteCore(message);
                return;
            }

            WriteToDebugger(message);

            if (s_shouldWriteToStdErr)
            {
                WriteToStderr(message);
            }
        }

        private static void WriteToDebugger(string message)
        {
            if (Debugger.IsLogging())
            {
                Debugger.Log(0, null, message);
            }
            else
            {
				throw new NotImplementedException();
            }
        }

        private static void WriteToStderr(string message)
        {
			throw new NotImplementedException();
        }
    }
}
