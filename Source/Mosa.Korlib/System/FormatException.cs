// Copyright (c) MOSA Project. Licensed under the New BSD License.
#nullable enable

//using System.Runtime.Serialization;

namespace System
{
	[Serializable]
	public class FormatException : Exception
	{
		public FormatException()
			: base(SR.Arg_FormatException)
		{
			HResult = HResults.COR_E_FORMAT;
		}

		public FormatException(string? message)
			: base(message)
		{
			HResult = HResults.COR_E_FORMAT;
		}

		public FormatException(string? message, Exception? innerException)
			: base(message, innerException)
		{
			HResult = HResults.COR_E_FORMAT;
		}

		//protected FormatException(SerializationInfo info, StreamingContext context) : base(info, context)
		//{ }
	}
}
