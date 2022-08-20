// Copyright (c) MOSA Project. Licensed under the New BSD License.
#nullable enable

using System.Runtime.Serialization;

namespace System
{
	[Serializable]
	public class DivideByZeroException : Exception
	{
		public DivideByZeroException()
			: base(SR.Arg_DivideByZero)
		{
			HResult = HResults.COR_E_DIVIDEBYZERO;
		}

		public DivideByZeroException(string? message)
			: base(message)
		{
			HResult = HResults.COR_E_DIVIDEBYZERO;
		}

		public DivideByZeroException(string? message, Exception? innerException)
			: base(message, innerException)
		{
			HResult = HResults.COR_E_DIVIDEBYZERO;
		}

		protected DivideByZeroException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{ }
	}
}
