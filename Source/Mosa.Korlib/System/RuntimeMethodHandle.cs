// Copyright (c) MOSA Project. Licensed under the New BSD License.

using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Runtime.CompilerServices;

namespace System
{
	[StructLayout(LayoutKind.Sequential)]
	public struct RuntimeMethodHandle : ISerializable
	{
		private IntPtr _value;

		public unsafe IntPtr Value => _value;

		public override bool Equals(object obj)
		{
			if (!(obj is RuntimeMethodHandle))
				return false;

			return Equals((RuntimeMethodHandle)obj);
		}

		public bool Equals(RuntimeMethodHandle handle)
		{
			if (_value == handle._value)
				return true;

			if (_value == IntPtr.Zero || handle._value == IntPtr.Zero)
				return false;

			return Equals(this, handle);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool Equals(RuntimeMethodHandle left, RuntimeMethodHandle right);

		public override int GetHashCode()
		{
			if (_value == IntPtr.Zero)
				return 0;
			return GetHashCode(this);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int GetHashCode(RuntimeMethodHandle handle);

		public static bool operator ==(RuntimeMethodHandle left, RuntimeMethodHandle right)
		{
			return left.Equals(right);
		}

		public static bool operator !=(RuntimeMethodHandle left, RuntimeMethodHandle right)
		{
			return !left.Equals(right);
		}

		public void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			throw new PlatformNotSupportedException();
		}
	}
}
