// Copyright (c) MOSA Project. Licensed under the New BSD License.

using System.Runtime.CompilerServices;

namespace System.Collections.Generic
{
	public abstract partial class EqualityComparer<T> : IEqualityComparer, IEqualityComparer<T>
	{
		// WARNING: We allow diagnostic tools to directly inspect this member (_default).
		// See https://github.com/dotnet/corert/blob/master/Documentation/design-docs/diagnostics/diagnostics-tools-contract.md for more details.
		// Please do not change the type, the name, or the semantic usage of this member without understanding the implication for tools.
		// Get in touch with the diagnostics team if you have questions.
		private static EqualityComparer<T> _default;

		[Intrinsic]
		private static EqualityComparer<T> Create()
		{
			// The compiler will overwrite the Create method with optimized
			// instantiation-specific implementation.
			throw new NotSupportedException();
		}

		public static EqualityComparer<T> Default
		{
			[Intrinsic]
			get
			{
				// Lazy initialization produces smaller code for CoreRT than initialization in constructor
				return _default ?? Create();
			}
		}
	}

	public sealed partial class EnumEqualityComparer<T> : EqualityComparer<T> where T : struct, Enum
	{
		public sealed override bool Equals(T x, T y)
		{
			return EnumOnlyEquals(x, y);
		}

		// This one is an intrinsic that is used to make enum comparisions more efficient.
		[Intrinsic]
		internal static bool EnumOnlyEquals<U>(U x, U y) where U : struct
		{
			return x.Equals(y);
		}
	}
}
