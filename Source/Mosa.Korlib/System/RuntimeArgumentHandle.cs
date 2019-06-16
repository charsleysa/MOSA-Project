// Copyright (c) MOSA Project. Licensed under the New BSD License.

using System.Runtime.InteropServices;

namespace System
{
	[StructLayout(LayoutKind.Sequential)]
	public ref struct RuntimeArgumentHandle
	{
		private IntPtr m_ptr;

		internal IntPtr Value { get { return m_ptr; } }
	}
}
