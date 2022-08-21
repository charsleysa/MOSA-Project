// Copyright (c) MOSA Project. Licensed under the New BSD License.
#nullable enable

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.Serialization;

namespace System.Reflection
{
	[Serializable]
	public sealed class AmbiguousMatchException : SystemException
	{
		public AmbiguousMatchException()
			: base(SR.RFLCT_Ambiguous)
		{
			HResult = HResults.COR_E_AMBIGUOUSMATCH;
		}

		public AmbiguousMatchException(string? message)
			: base(message)
		{
			HResult = HResults.COR_E_AMBIGUOUSMATCH;
		}

		public AmbiguousMatchException(string? message, Exception? inner)
			: base(message, inner)
		{
			HResult = HResults.COR_E_AMBIGUOUSMATCH;
		}

		private AmbiguousMatchException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{ }
	}
}
