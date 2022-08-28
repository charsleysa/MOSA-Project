// Copyright (c) MOSA Project. Licensed under the New BSD License.
#nullable enable

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.Serialization;

namespace System.Threading
{
	[Serializable]
	public class LockRecursionException : Exception
	{
		public LockRecursionException()
		{ }

		public LockRecursionException(string? message)
			: base(message)
		{ }

		public LockRecursionException(string? message, Exception? innerException)
			: base(message, innerException)
		{ }

		protected LockRecursionException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{ }
	}
}
