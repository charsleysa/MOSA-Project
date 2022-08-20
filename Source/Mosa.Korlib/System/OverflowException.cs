// Copyright (c) MOSA Project. Licensed under the New BSD License.
#nullable enable

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.Serialization;

namespace System
{
	[Serializable]
	public class OverflowException : Exception
	{
		public OverflowException()
			: base(SR.Arg_OverflowException)
		{
			HResult = HResults.COR_E_OVERFLOW;
		}

		public OverflowException(string? message)
			: base(message)
		{
			HResult = HResults.COR_E_OVERFLOW;
		}

		public OverflowException(string? message, Exception? innerException)
			: base(message, innerException)
		{
			HResult = HResults.COR_E_OVERFLOW;
		}

		protected OverflowException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{ }
	}
}
