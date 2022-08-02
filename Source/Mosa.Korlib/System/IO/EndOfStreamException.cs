// Copyright (c) MOSA Project. Licensed under the New BSD License.
#nullable enable

//using System.Runtime.Serialization;

namespace System
{
	/// <summary>
	/// The ArithmeticException is thrown when overflow or underflow occurs.
	/// </summary>
	[Serializable]
	public class EndOfStreamException : SystemException
	{
		public EndOfStreamException()
			: base(SR.Arg_EndOfStreamException)
		{
			HResult = HResults.COR_E_ENDOFSTREAM;
		}

		public EndOfStreamException(string? message)
			: base(message)
		{
			HResult = HResults.COR_E_ENDOFSTREAM;
		}

		public EndOfStreamException(string? message, Exception? innerException)
			: base(message, innerException)
		{
			HResult = HResults.COR_E_ENDOFSTREAM;
		}

		//protected EndOfStreamException(SerializationInfo info, StreamingContext context) : base(info, context)
		//{ }
	}
}
