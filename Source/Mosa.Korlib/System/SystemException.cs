// Copyright (c) MOSA Project. Licensed under the New BSD License.
#nullable enable

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.Serialization;

namespace System
{
	[Serializable]
	public class SystemException : Exception
	{
		public SystemException()
			: base(SR.Arg_SystemException)
		{
			HResult = HResults.COR_E_SYSTEM;
		}

		public SystemException(string? message)
			: base(message)
		{
			HResult = HResults.COR_E_SYSTEM;
		}

		public SystemException(string? message, Exception? innerException)
			: base(message, innerException)
		{
			HResult = HResults.COR_E_SYSTEM;
		}

		protected SystemException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{ }
	}
}
