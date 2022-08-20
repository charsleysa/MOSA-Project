// Copyright (c) MOSA Project. Licensed under the New BSD License.
#nullable enable

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.Serialization;

namespace System
{
	/// <summary>
	/// The ArithmeticException is thrown when overflow or underflow occurs.
	/// </summary>
	[Serializable]
	public class ArithmeticException : SystemException
	{
		public ArithmeticException()
			: base(SR.Arg_ArithmeticException)
		{
			HResult = HResults.COR_E_ARITHMETIC;
		}

		public ArithmeticException(string? message)
			: base(message)
		{
			HResult = HResults.COR_E_ARITHMETIC;
		}

		public ArithmeticException(string? message, Exception? innerException)
			: base(message, innerException)
		{
			HResult = HResults.COR_E_ARITHMETIC;
		}

		protected ArithmeticException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{ }
	}
}
