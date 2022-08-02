// Copyright (c) MOSA Project. Licensed under the New BSD License.
#nullable enable

namespace System
{
	/// <summary>
	/// Implementation of the "System.Exception" class
	/// </summary>
	[Serializable]
	public class Exception
	{
		internal string? message;
		private readonly Exception? innerException;
		private int _HResult;

		/// <summary>
		/// Gets a collection of key/value pairs that provide additional user-defined information about the exception.
		/// </summary>
		//public virtual IDictionary Data { get; }

		/// <summary>
		/// Retrieves the lowest exception (inner most) for the given Exception. This will traverse exceptions using the innerException property.
		/// </summary>
		public virtual Exception GetBaseException()
		{
			Exception? inner = InnerException;
			Exception back = this;

			while (inner != null)
			{
				back = inner;
				inner = inner.InnerException;
			}

			return back;
		}

		/// <summary>
		/// Gets the Exception instance that caused the current exception.
		/// </summary>
		public Exception? InnerException => innerException;

		/// <summary>
		/// Initializes a new instance of the <see cref="Exception"/> class.
		/// </summary>
		public Exception()
		{
			_HResult = HResults.COR_E_EXCEPTION;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Exception"/> class.
		/// </summary>
		/// <param name="message">The error message that explains the reason for the exception.</param>
		public Exception(string? message)
			: this()
		{
			this.message = message;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Exception"/> class with a specified error message and a reference to the inner exception that is the cause of this exception.
		/// </summary>
		/// <param name="message">The error message that explains the reason for the exception.</param>
		/// <param name="innerException">The exception that is the cause of the current exception, or a null reference if no inner exception is specified.</param>
		public Exception(string? message, Exception? innerException)
			: this()
		{
			this.message = message;
			this.innerException = innerException;
		}

		//protected Exception(SerializationInfo info, StreamingContext context)
		//{
		//	ArgumentNullException.ThrowIfNull(info);

		//	// TODO
		//}

		/// <summary>
		/// Gets the message.
		/// </summary>
		/// <value>The message.</value>
		public virtual string Message => message ?? SR.Format(SR.Exception_WasThrown, GetClassName());

		private string GetClassName() => GetType().ToString();

		public int HResult
		{
			get => _HResult;
			set => _HResult = value;
		}
	}
}
