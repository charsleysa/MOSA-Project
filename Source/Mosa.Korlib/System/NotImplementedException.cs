// Copyright (c) MOSA Project. Licensed under the New BSD License.
#nullable enable

//using System.Runtime.Serialization;

namespace System
{
	[Serializable]
	public class NotImplementedException : SystemException
	{
		public NotImplementedException()
			: base(SR.Arg_NotImplementedException)
		{
			HResult = HResults.E_NOTIMPL;
		}

		public NotImplementedException(string? message)
			: base(message)
		{
			HResult = HResults.E_NOTIMPL;
		}

		public NotImplementedException(string? message, Exception? innerException)
			: base(message, innerException)
		{
			HResult = HResults.E_NOTIMPL;
		}

		//protected NotImplementedException(SerializationInfo info, StreamingContext context) : base(info, context)
		//{ }
	}
}
