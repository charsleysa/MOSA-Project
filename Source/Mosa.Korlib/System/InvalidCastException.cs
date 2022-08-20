// Copyright (c) MOSA Project. Licensed under the New BSD License.
#nullable enable

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

/*=============================================================================
**
** Purpose: Exception class for invalid cast conditions!
**
=============================================================================*/

using System.Runtime.Serialization;

namespace System
{
	[Serializable]
	public class InvalidCastException : SystemException
	{
		public InvalidCastException()
			: base(SR.Arg_InvalidCastException)
		{
			HResult = HResults.COR_E_INVALIDCAST;
		}

		public InvalidCastException(string? message)
			: base(message)
		{
			HResult = HResults.COR_E_INVALIDCAST;
		}

		public InvalidCastException(string? message, Exception? innerException)
			: base(message, innerException)
		{
			HResult = HResults.COR_E_INVALIDCAST;
		}

		public InvalidCastException(string? message, int errorCode)
			: base(message)
		{
			HResult = errorCode;
		}

		protected InvalidCastException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{ }
	}
}
