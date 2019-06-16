// Copyright (c) MOSA Project. Licensed under the New BSD License.

using System.Collections;
using System.Globalization;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;

namespace System
{
	partial class Environment
	{
		public static int CurrentManagedThreadId => Thread.CurrentThread.ManagedThreadId;

		public extern static int ExitCode
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern bool HasShutdownStarted
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern int ProcessorCount
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static string StackTrace
		{
			[MethodImpl(MethodImplOptions.NoInlining)] // Prevent inlining from affecting where the stacktrace starts
			get => new StackTrace(true).ToString(System.Diagnostics.StackTrace.TraceFormat.Normal);
		}

		public extern static int TickCount
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern static long TickCount64
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern static void Exit(int exitCode);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern static string[] GetCommandLineArgs();

		public static void FailFast(string message)
		{
			throw new NotImplementedException();
		}

		public static void FailFast(string message, Exception exception)
		{
			throw new NotImplementedException();
		}

		public static void FailFast(string message, Exception exception, string errorMessage)
		{
			throw new NotImplementedException();
		}
	}
}
