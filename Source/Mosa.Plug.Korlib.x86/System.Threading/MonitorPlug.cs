// Copyright (c) MOSA Project. Licensed under the New BSD License.

using Mosa.Runtime.Plug;
using Mosa.Runtime.x86;
using System;
using System.Runtime.CompilerServices;

namespace Mosa.Plug.Korlib.System.Threading.x86
{
	public static class MonitorPlug
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		[Plug("System.Threading.Monitor::Enter")]
		internal static void Enter(object obj)
		{
			ArgumentNullException.ThrowIfNull(obj, nameof(obj));

			var sync = Runtime.Internal.GetObjectLockAndStatus(obj);

			while (Native.CmpXChgLoad32(sync.ToInt32(), 1, 0) != 0)
			{ }
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		[Plug("System.Threading.Monitor::ReliableEnter")]
		internal static void ReliableEnter(object obj, ref bool lockTaken)
		{
			ArgumentNullException.ThrowIfNull(obj, nameof(obj));

			var sync = Runtime.Internal.GetObjectLockAndStatus(obj);

			while (Native.CmpXChgLoad32(sync.ToInt32(), 1, 0) != 0)
			{ }

			lockTaken = true;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		[Plug("System.Threading.Monitor::Exit")]
		internal static void Exit(object obj)
		{
			ArgumentNullException.ThrowIfNull(obj, nameof(obj));

			var sync = Runtime.Internal.GetObjectLockAndStatus(obj);

			Native.XAddLoad32(sync.ToInt32(), -1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		[Plug("System.Threading.Monitor::IsEnteredNative")]
		internal static bool IsEnteredNative(object obj)
		{
			ArgumentNullException.ThrowIfNull(obj, nameof(obj));

			var sync = Runtime.Internal.GetObjectLockAndStatus(obj);

			return Native.Get32(sync.ToUInt32()) == 0;
		}
	}
}
