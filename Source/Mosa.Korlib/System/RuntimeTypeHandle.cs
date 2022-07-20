// Copyright (c) MOSA Project. Licensed under the New BSD License.

namespace System
{
	/// <summary>
	/// Represents a type using an internal metadata token.
	/// </summary>
	public struct RuntimeTypeHandle
	{
		internal RuntimeTypeHandle(IntPtr handle)
		{
			m_ptr = handle;
		}

		private readonly IntPtr m_ptr;

		/// <summary>
		/// Gets a handle to the type represented by this instance.
		/// </summary>
		public IntPtr Value { get { return m_ptr; } }

		public bool Equals(RuntimeTypeHandle obj)
		{
			return obj.m_ptr == m_ptr;
		}

		public override bool Equals(object obj)
		{
			if (!(obj is RuntimeTypeHandle))
				return false;

			var handle = (RuntimeTypeHandle)obj;

			return handle.m_ptr == m_ptr;
		}

		public static bool operator ==(RuntimeTypeHandle left, object right)
		{
			return left.Equals(right);
		}

		public static bool operator ==(object left, RuntimeTypeHandle right)
		{
			return right.Equals(left);
		}

		public static bool operator !=(RuntimeTypeHandle left, object right)
		{
			return !left.Equals(right);
		}

		public static bool operator !=(object left, RuntimeTypeHandle right)
		{
			return !right.Equals(left);
		}

		public override int GetHashCode()
		{
			return m_ptr.ToInt32();
		}
	}
}
