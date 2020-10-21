// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace System
{
    public static partial class Environment
    {
        public static string[] GetLogicalDrives() => throw new NotImplementedException();

        public static string MachineName
        {
            get
            {
				throw new NotImplementedException();
            }
        }

        public static long WorkingSet
        {
            get
            {
                Type? processType = Type.GetType("System.Diagnostics.Process, System.Diagnostics.Process, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", throwOnError: false);
                if (processType?.GetMethod("GetCurrentProcess")?.Invoke(null, BindingFlags.DoNotWrapExceptions, null, null, null) is IDisposable currentProcess)
                {
                    using (currentProcess)
                    {
                        if (processType!.GetMethod("get_WorkingSet64")?.Invoke(currentProcess, BindingFlags.DoNotWrapExceptions, null, null, null) is long result)
                            return result;
                    }
                }

                // Could not get the current working set.
                return 0;
            }
        }

        public static unsafe string UserName
        {
            get
            {
				throw new NotImplementedException();
			}
        }

        private static int GetCurrentProcessId() => throw new NotImplementedException();

		public static bool UserInteractive => true;

        private static string CurrentDirectoryCore
        {
            get => throw new NotImplementedException();
			set => throw new NotImplementedException();
		}

        private static string ExpandEnvironmentVariablesCore(string name)
        {
            var result = new ValueStringBuilder(stackalloc char[128]);

            int lastPos = 0, pos;
            while (lastPos < name.Length && (pos = name.IndexOf('%', lastPos + 1)) >= 0)
            {
                if (name[lastPos] == '%')
                {
                    string key = name.Substring(lastPos + 1, pos - lastPos - 1);
                    string? value = GetEnvironmentVariable(key);
                    if (value != null)
                    {
                        result.Append(value);
                        lastPos = pos + 1;
                        continue;
                    }
                }
                result.Append(name.AsSpan(lastPos, pos - lastPos));
                lastPos = pos;
            }
            result.Append(name.AsSpan(lastPos));

            return result.ToString();
        }

        private static bool Is64BitOperatingSystemWhen32BitProcess => false;

        internal const string NewLineConst = "\n";

        public static string SystemDirectory => throw new NotImplementedException();

		public static int SystemPageSize => throw new NotImplementedException();

		public static string UserDomainName => MachineName;
    }
}
