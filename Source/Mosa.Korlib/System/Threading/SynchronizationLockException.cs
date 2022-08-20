// Copyright (c) MOSA Project. Licensed under the New BSD License.
#nullable enable

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

/*=============================================================================
**
**
**
** Purpose: Wait(), Notify() or NotifyAll() was called from an unsynchronized
**          block of code.
**
**
=============================================================================*/

using System.Runtime.Serialization;

namespace System.Threading
{
	[Serializable]
	public class SynchronizationLockException : SystemException
	{
		public SynchronizationLockException()
			: base(SR.Arg_SynchronizationLockException)
		{
			HResult = HResults.COR_E_SYNCHRONIZATIONLOCK;
		}

		public SynchronizationLockException(string? message)
			: base(message)
		{
			HResult = HResults.COR_E_SYNCHRONIZATIONLOCK;
		}

		public SynchronizationLockException(string? message, Exception? innerException)
			: base(message, innerException)
		{
			HResult = HResults.COR_E_SYNCHRONIZATIONLOCK;
		}

		protected SynchronizationLockException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{ }
	}
}
