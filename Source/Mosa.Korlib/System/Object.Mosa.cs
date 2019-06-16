// Copyright (c) MOSA Project. Licensed under the New BSD License.

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

using Internal.Runtime.CompilerServices;

namespace System
{
	public partial class Object
	{
		// Returns a Type object which represent this object instance.
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern Type GetType();

		// Returns a new object instance that is a memberwise copy of this
		// object.  This is always a shallow copy of the instance. The method is protected
		// so that other object may only call this method on themselves.  It is intended to
		// support the ICloneable interface.
		[MethodImpl(MethodImplOptions.InternalCall)]
		protected extern object MemberwiseClone();

		[StructLayout(LayoutKind.Sequential)]
		private class RawData
		{
			public byte Data;
		}

		internal ref byte GetRawData()
		{
			return ref Unsafe.As<RawData>(this).Data;
		}
	}
}
