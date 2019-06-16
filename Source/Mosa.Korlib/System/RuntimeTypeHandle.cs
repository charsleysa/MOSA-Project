// Copyright (c) MOSA Project. Licensed under the New BSD License.

using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace System
{
	/// <summary>
	/// Represents a type using an internal metadata token.
	/// </summary>
	[StructLayout(LayoutKind.Sequential)]
	public struct RuntimeTypeHandle : IEquatable<RuntimeTypeHandle>, ISerializable
	{
		private IntPtr _value;

		public IntPtr Value => _value;

		public override bool Equals(object obj)
		{
			if (!(obj is RuntimeTypeHandle))
				return false;

			return Equals((RuntimeTypeHandle)obj);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool Equals(RuntimeTypeHandle handle)
		{
			if (_value == handle._value)
				return true;

			if (_value == IntPtr.Zero || handle._value == IntPtr.Zero)
				return false;

			return Equals(this, handle);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool Equals(RuntimeTypeHandle left, RuntimeTypeHandle right);

		public override int GetHashCode()
		{
			if (_value == IntPtr.Zero)
				return 0;
			return GetHashCode(this);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int GetHashCode(RuntimeTypeHandle handle);

		public static bool operator ==(RuntimeTypeHandle left, RuntimeTypeHandle right)
		{
			return left.Equals(right);
		}

		public static bool operator !=(RuntimeTypeHandle left, RuntimeTypeHandle right)
		{
			return !left.Equals(right);
		}

		public void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			throw new PlatformNotSupportedException();
		}
	}
}
