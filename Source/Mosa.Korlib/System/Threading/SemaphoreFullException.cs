// Copyright (c) MOSA Project. Licensed under the New BSD License.
#nullable enable

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.Serialization;

namespace System.Threading
{
	[Serializable]
	public class SemaphoreFullException : SystemException
	{
		public SemaphoreFullException()
			: base(SR.Threading_SemaphoreFullException)
		{ }

		public SemaphoreFullException(string? message)
			: base(message)
		{ }

		public SemaphoreFullException(string? message, Exception? innerException)
			: base(message, innerException)
		{ }

		protected SemaphoreFullException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{ }
	}
}
