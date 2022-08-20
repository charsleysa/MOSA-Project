// Copyright (c) MOSA Project. Licensed under the New BSD License.
#nullable enable

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.Serialization;

namespace System
{
	[Serializable]
	public class IndexOutOfRangeException : Exception
	{
		public IndexOutOfRangeException()
			: base(SR.Arg_IndexOutOfRangeException)
		{
			HResult = HResults.COR_E_INDEXOUTOFRANGE;
		}

		public IndexOutOfRangeException(string? message)
			: base(message)
		{
			HResult = HResults.COR_E_INDEXOUTOFRANGE;
		}

		public IndexOutOfRangeException(string? message, Exception? innerException)
			: base(message, innerException)
		{
			HResult = HResults.COR_E_INDEXOUTOFRANGE;
		}

		protected IndexOutOfRangeException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{ }
	}
}
