﻿// Copyright (c) MOSA Project. Licensed under the New BSD License.

//using System.Runtime.Serialization;

namespace System
{
	[Serializable]
	public class InvalidOperationException : SystemException
	{
		public InvalidOperationException()
			: base(SR.Arg_InvalidOperationException)
		{
			HResult = HResults.COR_E_INVALIDOPERATION;
		}

		public InvalidOperationException(string? message)
			: base(message)
		{
			HResult = HResults.COR_E_INVALIDOPERATION;
		}

		public InvalidOperationException(string? message, Exception? innerException)
			: base(message, innerException)
		{
			HResult = HResults.COR_E_INVALIDOPERATION;
		}

		//protected InvalidOperationException(SerializationInfo info, StreamingContext context) : base(info, context)
		//{ }
	}
}
