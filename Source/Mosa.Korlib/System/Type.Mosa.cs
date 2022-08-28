// Copyright (c) MOSA Project. Licensed under the New BSD License.
#nullable enable

using System.Reflection;
using System.Runtime.CompilerServices;

namespace System
{
	public abstract partial class Type
	{
		public bool IsInterface => (GetAttributeFlagsImpl() & TypeAttributes.ClassSemanticsMask) == TypeAttributes.Interface;

		[Intrinsic]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern bool IsRuntimeImplemented();
	}
}
