// Copyright (c) MOSA Project. Licensed under the New BSD License.
#nullable enable

//using System.Runtime.Serialization;

namespace System
{
	[Serializable]
	public class PlatformNotSupportedException : Exception
	{
		public PlatformNotSupportedException()
			: base(SR.Arg_PlatformNotSupported)
		{
			HResult = HResults.COR_E_PLATFORMNOTSUPPORTED;
		}

		public PlatformNotSupportedException(string? message)
			: base(message)
		{
			HResult = HResults.COR_E_PLATFORMNOTSUPPORTED;
		}

		public PlatformNotSupportedException(string? message, Exception? innerException)
			: base(message, innerException)
		{
			HResult = HResults.COR_E_PLATFORMNOTSUPPORTED;
		}

		//protected PlatformNotSupportedException(SerializationInfo info, StreamingContext context) : base(info, context)
		//{ }
	}
}
