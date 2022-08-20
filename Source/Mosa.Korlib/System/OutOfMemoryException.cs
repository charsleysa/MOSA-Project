// Copyright (c) MOSA Project. Licensed under the New BSD License.
#nullable enable

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.Serialization;

namespace System
{
	/// <summary>
	/// The exception class for OOM.
	/// </summary>
	[Serializable]
	public class OutOfMemoryException : SystemException
	{
		public OutOfMemoryException()
			: base(SR.Arg_OutOfMemoryException)
		{
			HResult = HResults.COR_E_OUTOFMEMORY;
		}

		public OutOfMemoryException(string? message)
			: base(message)
		{
			HResult = HResults.COR_E_OUTOFMEMORY;
		}

		public OutOfMemoryException(string? message, Exception? innerException)
			: base(message, innerException)
		{
			HResult = HResults.COR_E_OUTOFMEMORY;
		}

		protected OutOfMemoryException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{ }
	}
}
