// Copyright (c) MOSA Project. Licensed under the New BSD License.
#nullable enable

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.Serialization;

namespace System
{
	[Serializable]
	public class NotImplementedException : SystemException
	{
		public NotImplementedException()
			: base(SR.Arg_NotImplementedException)
		{
			HResult = HResults.E_NOTIMPL;
		}

		public NotImplementedException(string? message)
			: base(message)
		{
			HResult = HResults.E_NOTIMPL;
		}

		public NotImplementedException(string? message, Exception? innerException)
			: base(message, innerException)
		{
			HResult = HResults.E_NOTIMPL;
		}

		protected NotImplementedException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{ }
	}
}
