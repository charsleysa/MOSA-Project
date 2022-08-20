// Copyright (c) MOSA Project. Licensed under the New BSD License.

using Mosa.Runtime.Plug;
using Mosa.Runtime.x64;
using System;
using System.Runtime.CompilerServices;

namespace Mosa.Plug.Korlib.System.Threading.x64
{
	public static class MonitorPlug
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		[Plug("System.Threading.Monitor::Enter")]
		internal static void Enter(object obj)
		{
			ArgumentNullException.ThrowIfNull(obj, nameof(obj));

			var sync = Runtime.Internal.GetObjectLockAndStatus(obj);

			while (Native.CmpXChgLoad64(sync.ToInt64(), 1, 0) != 0)
			{ }
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		[Plug("System.Threading.Monitor::ReliableEnter")]
		internal static void ReliableEnter(object obj, ref bool lockTaken)
		{
			ArgumentNullException.ThrowIfNull(obj, nameof(obj));

			var sync = Runtime.Internal.GetObjectLockAndStatus(obj);

			while (Native.CmpXChgLoad64(sync.ToInt64(), 1, 0) != 0)
			{ }

			lockTaken = true;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		[Plug("System.Threading.Monitor::Exit")]
		internal static void Exit(object obj)
		{
			ArgumentNullException.ThrowIfNull(obj, nameof(obj));

			var sync = Runtime.Internal.GetObjectLockAndStatus(obj);

			Native.XAddLoad64(sync.ToInt64(), -1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		[Plug("System.Threading.Monitor::IsEnteredNative")]
		internal static bool IsEnteredNative(object obj)
		{
			ArgumentNullException.ThrowIfNull(obj, nameof(obj));

			var sync = Runtime.Internal.GetObjectLockAndStatus(obj);

			return Native.Get64(sync.ToUInt64()) == 0;
		}
	}
}
