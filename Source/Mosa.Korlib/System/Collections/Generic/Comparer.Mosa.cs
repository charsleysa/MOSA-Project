﻿// Copyright (c) MOSA Project. Licensed under the New BSD License.

using System.Runtime.CompilerServices;

namespace System.Collections.Generic
{
	public abstract partial class Comparer<T> : IComparer, IComparer<T>
	{
		// WARNING: We allow diagnostic tools to directly inspect this member (_default).
		// See https://github.com/dotnet/corert/blob/master/Documentation/design-docs/diagnostics/diagnostics-tools-contract.md for more details.
		// Please do not change the type, the name, or the semantic usage of this member without understanding the implication for tools.
		// Get in touch with the diagnostics team if you have questions.
		private static Comparer<T> _default;

		[Intrinsic]
		private static Comparer<T> Create()
		{
			// The compiler will overwrite the Create method with optimized
			// instantiation-specific implementation.
			throw new NotSupportedException();
		}

		public static Comparer<T> Default
		{
			get
			{
				// Lazy initialization produces smaller code for CoreRT than initialization in constructor
				return _default ?? Create();
			}
		}
	}

	internal sealed partial class EnumComparer<T> : Comparer<T> where T : struct, Enum
	{
		public override int Compare(T x, T y)
		{
			// CORERT-TODO: EnumComparer<T>
			throw new NotImplementedException();
		}
	}
}
