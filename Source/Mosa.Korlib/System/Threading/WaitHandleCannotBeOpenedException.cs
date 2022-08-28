// Copyright (c) MOSA Project. Licensed under the New BSD License.
#nullable enable

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.Serialization;

namespace System.Threading
{
	[Serializable]
	public class WaitHandleCannotBeOpenedException : ApplicationException
	{
		public WaitHandleCannotBeOpenedException()
			: base(SR.Threading_WaitHandleCannotBeOpenedException)
		{
			HResult = HResults.COR_E_WAITHANDLECANNOTBEOPENED;
		}

		public WaitHandleCannotBeOpenedException(string? message)
			: base(message)
		{
			HResult = HResults.COR_E_WAITHANDLECANNOTBEOPENED;
		}

		public WaitHandleCannotBeOpenedException(string? message, Exception? innerException)
			: base(message, innerException)
		{
			HResult = HResults.COR_E_WAITHANDLECANNOTBEOPENED;
		}

		protected WaitHandleCannotBeOpenedException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{ }
	}
}
