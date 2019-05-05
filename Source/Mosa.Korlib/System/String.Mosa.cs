// Copyright (c) MOSA Project. Licensed under the New BSD License.

using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Runtime;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Text;

using Internal.Runtime.CompilerServices;

namespace System
{
	/// <summary>
	/// Implementation of the "System.String" class
	/// </summary>
	[StructLayout(LayoutKind.Sequential)]
	[System.Runtime.CompilerServices.EagerStaticClassConstructionAttribute]
	public partial class String
	{
		// WARNING: We allow diagnostic tools to directly inspect these two members (_stringLength, _firstChar)
		// See https://github.com/dotnet/corert/blob/master/Documentation/design-docs/diagnostics/diagnostics-tools-contract.md for more details.
		// Please do not change the type, the name, or the semantic usage of this member without understanding the implication for tools.
		// Get in touch with the diagnostics team if you have questions.
		[NonSerialized]
		private int _stringLength;

		[NonSerialized]
		private char _firstChar;

		public static readonly string Empty = "";

		public int Length { get { return _stringLength; } }

		[IndexerName("Chars")]
		public unsafe char this[int index]
		{
			get
			{
				if ((uint)index >= _stringLength)
					ThrowHelper.ThrowIndexOutOfRangeException();
				return Unsafe.Add(ref _firstChar, index);
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern string InternalAllocateString(int length);

		internal static string FastAllocateString(int length)
		{
			string newStr = InternalAllocateString(length);
			Debug.Assert(newStr._stringLength == length);
			return newStr;
		}
	}
}
