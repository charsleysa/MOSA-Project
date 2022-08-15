// Copyright (c) MOSA Project. Licensed under the New BSD License.
#nullable enable

using System.Runtime.CompilerServices;

namespace System.Runtime.InteropServices
{
	public partial struct GCHandle
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr InternalAlloc(object? value, GCHandleType type);

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void InternalFree(IntPtr handle);

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern object? InternalGet(IntPtr handle);

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void InternalSet(IntPtr handle, object? value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern object? InternalCompareExchange(IntPtr handle, object? value, object? oldValue);
	}
}
