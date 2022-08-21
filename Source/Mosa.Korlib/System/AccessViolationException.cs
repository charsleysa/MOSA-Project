// Copyright (c) MOSA Project. Licensed under the New BSD License.
#nullable enable

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

/*=============================================================================
**
**
**
** Purpose: Exception class representing an AV that was deemed unsafe and may have corrupted the application.
**
**
=============================================================================*/

using System.Runtime.Serialization;

namespace System
{
	[Serializable]
	public class AccessViolationException : SystemException
	{
		public AccessViolationException()
			: base(SR.Arg_AccessViolationException)
		{
			HResult = HResults.E_POINTER;
		}

		public AccessViolationException(string? message)
			: base(message)
		{
			HResult = HResults.E_POINTER;
		}

		public AccessViolationException(string? message, Exception? innerException)
			: base(message, innerException)
		{
			HResult = HResults.E_POINTER;
		}

		protected AccessViolationException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{ }

#pragma warning disable CA1823, 169 // Field is not used from managed.
		private IntPtr _ip;             // Address of faulting instruction.
		private IntPtr _target;         // Address that could not be accessed.
		private int _accessType;        // 0:read, 1:write
#pragma warning restore CA1823, 169
	}
}
