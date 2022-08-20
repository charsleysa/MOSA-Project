// Copyright (c) MOSA Project. Licensed under the New BSD License.

using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace System.Threading
{
	public static class Monitor
	{
		[MethodImplAttribute(MethodImplOptions.InternalCall)]
		public static extern void Enter(object obj);

		public static void Enter(object obj, ref bool lockTaken)
		{
			if (lockTaken)
				ThrowLockTakenException();

			ReliableEnter(obj, ref lockTaken);
			Debug.Assert(lockTaken);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ReliableEnter(object obj, ref bool lockTaken);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void Exit(object obj);

		[DoesNotReturn]
		private static void ThrowLockTakenException()
		{
			throw new ArgumentException(SR.Argument_MustBeFalse, "lockTaken");
		}

		public static bool IsEntered(object obj)
		{
			if (obj == null)
				throw new ArgumentNullException(nameof(obj));

			return IsEnteredNative(obj);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool IsEnteredNative(object obj);
	}
}
