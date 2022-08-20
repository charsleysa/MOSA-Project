// Copyright (c) MOSA Project. Licensed under the New BSD License.
#nullable enable

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Runtime.Serialization
{
	[Serializable]
	public class SerializationException : SystemException
	{
		/// <summary>
		/// Creates a new SerializationException with its message
		/// string set to a default message.
		/// </summary>
		public SerializationException()
			: base(SR.SerializationException)
		{
			HResult = HResults.COR_E_SERIALIZATION;
		}

		public SerializationException(string? message)
			: base(message)
		{
			HResult = HResults.COR_E_SERIALIZATION;
		}

		public SerializationException(string? message, Exception? innerException)
			: base(message, innerException)
		{
			HResult = HResults.COR_E_SERIALIZATION;
		}

		protected SerializationException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
