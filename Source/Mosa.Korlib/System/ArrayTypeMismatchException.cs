// Copyright (c) MOSA Project. Licensed under the New BSD License.
#nullable enable

using System.Runtime.Serialization;

namespace System
{
	/// <summary>
	/// The ArrayMismatchException is thrown when an attempt to store an object of the wrong type within an array occurs.
	/// </summary>
	[Serializable]
	public class ArrayTypeMismatchException : SystemException
	{
		public ArrayTypeMismatchException()
			: base(SR.Arg_ArrayTypeMismatchException)
		{
			HResult = HResults.COR_E_ARRAYTYPEMISMATCH;
		}

		public ArrayTypeMismatchException(string? message)
			: base(message)
		{
			HResult = HResults.COR_E_ARRAYTYPEMISMATCH;
		}

		public ArrayTypeMismatchException(string? message, Exception? innerException)
			: base(message, innerException)
		{
			HResult = HResults.COR_E_ARRAYTYPEMISMATCH;
		}

		protected ArrayTypeMismatchException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{ }
	}
}
