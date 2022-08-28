// Copyright (c) MOSA Project. Licensed under the New BSD License.
#nullable enable

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

/*=============================================================================
**
**
**
** Purpose: Exception class for Timeout
**
**
=============================================================================*/

using System.Runtime.Serialization;

namespace System
{
	[Serializable]
	public class TimeoutException : SystemException
	{
		public TimeoutException()
			: base(SR.Arg_TimeoutException)
		{
			HResult = HResults.COR_E_TIMEOUT;
		}

		public TimeoutException(string? message)
			: base(message)
		{
			HResult = HResults.COR_E_TIMEOUT;
		}

		public TimeoutException(string? message, Exception? innerException)
			: base(message, innerException)
		{
			HResult = HResults.COR_E_TIMEOUT;
		}

		protected TimeoutException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{ }
	}
}
