// Copyright (c) MOSA Project. Licensed under the New BSD License.

using System.Runtime.CompilerServices;

using nuint = System.UInt64;

namespace System.Runtime
{
	internal static class RuntimeImports
	{
		internal static unsafe void RhZeroMemory(ref byte b, nuint byteLength)
		{
			fixed (byte* bytePointer = &b)
			{
				ZeroMemory(bytePointer, byteLength);
			}
		}

		internal static unsafe void RhZeroMemory(IntPtr p, UIntPtr byteLength) => ZeroMemory((void*)p, (nuint)byteLength);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern unsafe void ZeroMemory(void* p, nuint byteLength);

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern static void RhBulkMoveWithWriteBarrier(ref byte dmem, ref byte smem, nuint size);
	}
}
