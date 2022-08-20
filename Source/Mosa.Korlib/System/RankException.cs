﻿// Copyright (c) MOSA Project. Licensed under the New BSD License.
#nullable enable

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.Serialization;

namespace System
{
	[Serializable]
	public class RankException : Exception
	{
		public RankException()
			: base(SR.Arg_RankException)
		{
			HResult = HResults.COR_E_RANK;
		}

		public RankException(string? message)
			: base(message)
		{
			HResult = HResults.COR_E_RANK;
		}

		public RankException(string? message, Exception? innerException)
			: base(message, innerException)
		{
			HResult = HResults.COR_E_RANK;
		}

		protected RankException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{ }
	}
}
