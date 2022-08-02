// Copyright (c) MOSA Project. Licensed under the New BSD License.
#nullable enable

//using System.Runtime.Serialization;

namespace System
{
	public class NullReferenceException : Exception
	{
		public NullReferenceException()
			: base(SR.Arg_NullReferenceException)
		{
			HResult = HResults.E_POINTER;
		}

		public NullReferenceException(string? message)
			: base(message)
		{
			HResult = HResults.E_POINTER;
		}

		public NullReferenceException(string? message, Exception? innerException)
			: base(message, innerException)
		{
			HResult = HResults.E_POINTER;
		}

		//protected NullReferenceException(SerializationInfo info, StreamingContext context) : base(info, context)
		//{ }
	}
}
