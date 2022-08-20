// Copyright (c) MOSA Project. Licensed under the New BSD License.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#if TARGET_AMD64 || TARGET_ARM64 || (TARGET_32BIT && !TARGET_ARM)
#define HAS_CUSTOM_BLOCKS
#endif

using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace System
{
	public static partial class Buffer
	{
		private const nuint MemmoveNativeThreshold = 512;

		// The maximum block size to for __BulkMoveWithWriteBarrier FCall. This is required to avoid GC starvation.
#if DEBUG // Stress the mechanism in debug builds
		private const uint BulkMoveWithWriteBarrierChunk = 0x400;
#else
		private const uint BulkMoveWithWriteBarrierChunk = 0x4000;
#endif

		// Copies from one primitive array to another primitive array without
		// respecting types.  This calls memmove internally.  The count and
		// offset parameters here are in bytes.  If you want to use traditional
		// array element indices and counts, use Array.Copy.
		public static unsafe void BlockCopy(Array src, int srcOffset, Array dst, int dstOffset, int count)
		{
			if (src == null)
				throw new ArgumentNullException(nameof(src));
			if (dst == null)
				throw new ArgumentNullException(nameof(dst));

			nuint uSrcLen = src.NativeLength;
			if (src.GetType() != typeof(byte[]))
			{
				if (!src.GetCorElementTypeOfElementType().IsPrimitiveType())
					throw new ArgumentException(SR.Arg_MustBePrimArray, nameof(src));
				uSrcLen *= (nuint)src.GetElementSize();
			}

			nuint uDstLen = uSrcLen;
			if (src != dst)
			{
				uDstLen = dst.NativeLength;
				if (dst.GetType() != typeof(byte[]))
				{
					if (!dst.GetCorElementTypeOfElementType().IsPrimitiveType())
						throw new ArgumentException(SR.Arg_MustBePrimArray, nameof(dst));
					uDstLen *= (nuint)dst.GetElementSize();
				}
			}

			if (srcOffset < 0)
				throw new ArgumentOutOfRangeException(nameof(srcOffset), SR.ArgumentOutOfRange_MustBeNonNegInt32);
			if (dstOffset < 0)
				throw new ArgumentOutOfRangeException(nameof(dstOffset), SR.ArgumentOutOfRange_MustBeNonNegInt32);
			if (count < 0)
				throw new ArgumentOutOfRangeException(nameof(count), SR.ArgumentOutOfRange_MustBeNonNegInt32);

			nuint uCount = (nuint)count;
			nuint uSrcOffset = (nuint)srcOffset;
			nuint uDstOffset = (nuint)dstOffset;

			if ((uSrcLen < uSrcOffset + uCount) || (uDstLen < uDstOffset + uCount))
				throw new ArgumentException(SR.Argument_InvalidOffLen);

			Memmove(ref Unsafe.AddByteOffset(ref MemoryMarshal.GetArrayDataReference(dst), uDstOffset), ref Unsafe.AddByteOffset(ref MemoryMarshal.GetArrayDataReference(src), uSrcOffset), uCount);
		}

		public static int ByteLength(Array array)
		{
			// Is the array present?
			if (array == null)
				throw new ArgumentNullException(nameof(array));

			// Is it of primitive types?
			if (!array.GetCorElementTypeOfElementType().IsPrimitiveType())
				throw new ArgumentException(SR.Arg_MustBePrimArray, nameof(array));

			nuint byteLength = array.NativeLength * (nuint)array.GetElementSize();

			// This API is explosed both as Buffer.ByteLength and also used indirectly in argument
			// checks for Buffer.GetByte/SetByte.
			//
			// If somebody called Get/SetByte on 2GB+ arrays, there is a decent chance that
			// the computation of the index has overflowed. Thus we intentionally always
			// throw on 2GB+ arrays in Get/SetByte argument checks (even for indicies <2GB)
			// to prevent people from running into a trap silently.

			return checked((int)byteLength);
		}

		public static byte GetByte(Array array, int index)
		{
			// array argument validation done via ByteLength
			if ((uint)index >= (uint)ByteLength(array))
			{
				ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.index);
			}

			return Unsafe.Add<byte>(ref MemoryMarshal.GetArrayDataReference(array), index);
		}

		public static void SetByte(Array array, int index, byte value)
		{
			// array argument validation done via ByteLength
			if ((uint)index >= (uint)ByteLength(array))
			{
				ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.index);
			}

			Unsafe.Add<byte>(ref MemoryMarshal.GetArrayDataReference(array), index) = value;
		}

		internal static unsafe void ZeroMemory(byte* dest, nuint len)
		{
			SpanHelpers.ClearWithoutReferences(ref *dest, len);
		}

		// The attributes on this method are chosen for best JIT performance.
		// Please do not edit unless intentional.
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		[CLSCompliant(false)]
		public static unsafe void MemoryCopy(void* source, void* destination, long destinationSizeInBytes, long sourceBytesToCopy)
		{
			if (sourceBytesToCopy > destinationSizeInBytes)
			{
				ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.sourceBytesToCopy);
			}

			Memmove(ref *(byte*)destination, ref *(byte*)source, checked((nuint)sourceBytesToCopy));
		}

		// The attributes on this method are chosen for best JIT performance.
		// Please do not edit unless intentional.
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		[CLSCompliant(false)]
		public static unsafe void MemoryCopy(void* source, void* destination, ulong destinationSizeInBytes, ulong sourceBytesToCopy)
		{
			if (sourceBytesToCopy > destinationSizeInBytes)
			{
				ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.sourceBytesToCopy);
			}

			Memmove(ref *(byte*)destination, ref *(byte*)source, checked((nuint)sourceBytesToCopy));
		}

		internal static void Memmove(ref byte dest, ref byte src, nuint len)
		{
			// P/Invoke into the native version when the buffers are overlapping.
			if (((nuint)(nint)Unsafe.ByteOffset(ref src, ref dest) < len) || ((nuint)(nint)Unsafe.ByteOffset(ref dest, ref src) < len))
			{
				goto BuffersOverlap;
			}

			// Use "(IntPtr)(nint)len" to avoid overflow checking on the explicit cast to IntPtr

			ref byte srcEnd = ref Unsafe.Add(ref src, (IntPtr)(nint)len);
			ref byte destEnd = ref Unsafe.Add(ref dest, (IntPtr)(nint)len);

			if (len <= 16)
				goto MCPY02;
			if (len > 64)
				goto MCPY05;

			MCPY00:

			// Copy bytes which are multiples of 16 and leave the remainder for MCPY01 to handle.
			Debug.Assert(len > 16 && len <= 64);
			Unsafe.As<byte, Block16>(ref dest) = Unsafe.As<byte, Block16>(ref src); // [0,16]
			if (len <= 32)
				goto MCPY01;
			Unsafe.As<byte, Block16>(ref Unsafe.Add(ref dest, 16)) = Unsafe.As<byte, Block16>(ref Unsafe.Add(ref src, 16)); // [0,32]
			if (len <= 48)
				goto MCPY01;
			Unsafe.As<byte, Block16>(ref Unsafe.Add(ref dest, 32)) = Unsafe.As<byte, Block16>(ref Unsafe.Add(ref src, 32)); // [0,48]

		MCPY01:

			// Unconditionally copy the last 16 bytes using destEnd and srcEnd and return.
			Debug.Assert(len > 16 && len <= 64);
			Unsafe.As<byte, Block16>(ref Unsafe.Add(ref destEnd, -16)) = Unsafe.As<byte, Block16>(ref Unsafe.Add(ref srcEnd, -16));
			return;

		MCPY02:

			// Copy the first 8 bytes and then unconditionally copy the last 8 bytes and return.
			if ((len & 24) == 0)
				goto MCPY03;
			Debug.Assert(len >= 8 && len <= 16);
			Unsafe.As<byte, long>(ref dest) = Unsafe.As<byte, long>(ref src);
			Unsafe.As<byte, long>(ref Unsafe.Add(ref destEnd, -8)) = Unsafe.As<byte, long>(ref Unsafe.Add(ref srcEnd, -8));
			return;

		MCPY03:

			// Copy the first 4 bytes and then unconditionally copy the last 4 bytes and return.
			if ((len & 4) == 0)
				goto MCPY04;
			Debug.Assert(len >= 4 && len < 8);
			Unsafe.As<byte, int>(ref dest) = Unsafe.As<byte, int>(ref src);
			Unsafe.As<byte, int>(ref Unsafe.Add(ref destEnd, -4)) = Unsafe.As<byte, int>(ref Unsafe.Add(ref srcEnd, -4));
			return;

		MCPY04:

			// Copy the first byte. For pending bytes, do an unconditionally copy of the last 2 bytes and return.
			Debug.Assert(len < 4);
			if (len == 0)
				return;
			dest = src;
			if ((len & 2) == 0)
				return;
			Unsafe.As<byte, short>(ref Unsafe.Add(ref destEnd, -2)) = Unsafe.As<byte, short>(ref Unsafe.Add(ref srcEnd, -2));
			return;

		MCPY05:

			// PInvoke to the native version when the copy length exceeds the threshold.
			if (len > MemmoveNativeThreshold)
			{
				goto PInvoke;
			}

			// Copy 64-bytes at a time until the remainder is less than 64.
			// If remainder is greater than 16 bytes, then jump to MCPY00. Otherwise, unconditionally copy the last 16 bytes and return.
			Debug.Assert(len > 64 && len <= MemmoveNativeThreshold);
			nuint n = len >> 6;

		MCPY06:
			Unsafe.As<byte, Block64>(ref dest) = Unsafe.As<byte, Block64>(ref src);
			dest = ref Unsafe.Add(ref dest, 64);
			src = ref Unsafe.Add(ref src, 64);
			n--;
			if (n != 0)
				goto MCPY06;

			len %= 64;
			if (len > 16)
				goto MCPY00;
			Unsafe.As<byte, Block16>(ref Unsafe.Add(ref destEnd, -16)) = Unsafe.As<byte, Block16>(ref Unsafe.Add(ref srcEnd, -16));
			return;

		BuffersOverlap:

			// If the buffers overlap perfectly, there's no point to copying the data.
			if (Unsafe.AreSame(ref dest, ref src))
			{
				return;
			}

		PInvoke:
			_Memmove(ref dest, ref src, len);
		}

		// Non-inlinable wrapper around the QCall that avoids polluting the fast path
		// with P/Invoke prolog/epilog.
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern unsafe void _Memmove(ref byte dest, ref byte src, nuint len);

		// Used by ilmarshalers.cpp
		internal static unsafe void Memcpy(byte* dest, byte* src, int len)
		{
			Debug.Assert(len >= 0, "Negative length in memcpy!");
			Memmove(ref *dest, ref *src, (nuint)(uint)len /* force zero-extension */);
		}

		// Used by ilmarshalers.cpp
		internal static unsafe void Memcpy(byte* pDest, int destIndex, byte[] src, int srcIndex, int len)
		{
			Debug.Assert((srcIndex >= 0) && (destIndex >= 0) && (len >= 0), "Index and length must be non-negative!");
			Debug.Assert(src.Length - srcIndex >= len, "not enough bytes in src");

			Memmove(ref *(pDest + (uint)destIndex), ref Unsafe.Add(ref MemoryMarshal.GetArrayDataReference(src), (nint)(uint)srcIndex /* force zero-extension */), (uint)len);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static void Memmove<T>(ref T destination, ref T source, nuint elementCount)
		{
			if (!RuntimeHelpers.IsReferenceOrContainsReferences<T>())
			{
				// Blittable memmove
				Memmove(
					ref Unsafe.As<T, byte>(ref destination),
					ref Unsafe.As<T, byte>(ref source),
					elementCount * (nuint)Unsafe.SizeOf<T>());
			}
			else
			{
				// Non-blittable memmove
				BulkMoveWithWriteBarrier(
					ref Unsafe.As<T, byte>(ref destination),
					ref Unsafe.As<T, byte>(ref source),
					elementCount * (nuint)Unsafe.SizeOf<T>());
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern unsafe void _ZeroMemory(ref byte b, nuint byteLength);

		internal static void BulkMoveWithWriteBarrier(ref byte destination, ref byte source, nuint byteCount)
		{
			if (byteCount <= BulkMoveWithWriteBarrierChunk)
				__BulkMoveWithWriteBarrier(ref destination, ref source, byteCount);
			else
				_BulkMoveWithWriteBarrier(ref destination, ref source, byteCount);
		}

		// Non-inlinable wrapper around the loop for copying large blocks in chunks
		[MethodImpl(MethodImplOptions.NoInlining)]
		private static void _BulkMoveWithWriteBarrier(ref byte destination, ref byte source, nuint byteCount)
		{
			Debug.Assert(byteCount > BulkMoveWithWriteBarrierChunk);

			if (Unsafe.AreSame(ref source, ref destination))
				return;

			// This is equivalent to: (destination - source) >= byteCount || (destination - source) < 0
			if ((nuint)(nint)Unsafe.ByteOffset(ref source, ref destination) >= byteCount)
			{
				// Copy forwards
				do
				{
					byteCount -= BulkMoveWithWriteBarrierChunk;
					__BulkMoveWithWriteBarrier(ref destination, ref source, BulkMoveWithWriteBarrierChunk);
					destination = ref Unsafe.AddByteOffset(ref destination, BulkMoveWithWriteBarrierChunk);
					source = ref Unsafe.AddByteOffset(ref source, BulkMoveWithWriteBarrierChunk);
				}
				while (byteCount > BulkMoveWithWriteBarrierChunk);
			}
			else
			{
				// Copy backwards
				do
				{
					byteCount -= BulkMoveWithWriteBarrierChunk;
					__BulkMoveWithWriteBarrier(ref Unsafe.AddByteOffset(ref destination, byteCount), ref Unsafe.AddByteOffset(ref source, byteCount), BulkMoveWithWriteBarrierChunk);
				}
				while (byteCount > BulkMoveWithWriteBarrierChunk);
			}
			__BulkMoveWithWriteBarrier(ref destination, ref source, byteCount);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void __BulkMoveWithWriteBarrier(ref byte destination, ref byte source, nuint byteCount);

		[StructLayout(LayoutKind.Sequential, Size = 16)]
		private struct Block16 { }

		[StructLayout(LayoutKind.Sequential, Size = 64)]
		private struct Block64 { }
	}
}
