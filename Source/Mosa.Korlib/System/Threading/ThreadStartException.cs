// Copyright (c) MOSA Project. Licensed under the New BSD License.
#nullable enable

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.Serialization;

namespace System.Threading
{
	[Serializable]
	public sealed class ThreadStartException : SystemException
	{
		internal ThreadStartException()
			: base(SR.Arg_ThreadStartException)
		{
			HResult = HResults.COR_E_THREADSTART;
		}

		internal ThreadStartException(Exception reason)
			: base(SR.Arg_ThreadStartException, reason)
		{
			HResult = HResults.COR_E_THREADSTART;
		}

		private ThreadStartException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{ }
	}
}
