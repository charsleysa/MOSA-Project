// Copyright (c) MOSA Project. Licensed under the New BSD License.
#nullable enable

//using System.Runtime.Serialization;

namespace System
{
	[Serializable]
	public class NotSupportedException : Exception
	{
		public NotSupportedException()
			: base(SR.Arg_NotSupportedException)
		{
			HResult = HResults.COR_E_NOTSUPPORTED;
		}

		public NotSupportedException(string? message)
			: base(message)
		{
			HResult = HResults.COR_E_NOTSUPPORTED;
		}

		public NotSupportedException(string? message, Exception? innerException)
			: base(message, innerException)
		{
			HResult = HResults.COR_E_NOTSUPPORTED;
		}

		//protected NotSupportedException(SerializationInfo info, StreamingContext context) : base(info, context)
		//{ }
	}
}
