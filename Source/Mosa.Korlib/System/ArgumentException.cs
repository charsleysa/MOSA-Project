﻿// Copyright (c) MOSA Project. Licensed under the New BSD License.
#nullable enable

using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

//using System.Runtime.Serialization;

namespace System
{
	/// <summary>
	/// The ArgumentException is thrown when an argument does not meet the contract of the method. Ideally it should give a meaningful error message describing what was wrong and which parameter is incorrect.
	/// </summary>
	[Serializable]
	public class ArgumentException : SystemException
	{
		private readonly string? _paramName;

		/// <summary>
		/// Creates a new ArgumentException with its message string set to the empty string.
		/// </summary>
		public ArgumentException()
			: base(SR.Arg_ArgumentException)
		{
			HResult = HResults.COR_E_ARGUMENT;
		}

		/// <summary>
		/// Creates a new ArgumentException with its message string set to message.
		/// </summary>
		/// <param name="message"></param>
		public ArgumentException(string? message)
			: base(message)
		{
			HResult = HResults.COR_E_ARGUMENT;
		}

		public ArgumentException(string? message, Exception? innerException)
			: base(message, innerException)
		{
			HResult = HResults.COR_E_ARGUMENT;
		}

		public ArgumentException(string? message, string? paramName, Exception? innerException)
			: base(message, innerException)
		{
			_paramName = paramName;
			HResult = HResults.COR_E_ARGUMENT;
		}

		public ArgumentException(string? message, string? paramName)
			: base(message)
		{
			_paramName = paramName;
			HResult = HResults.COR_E_ARGUMENT;
		}

		//protected ArgumentException(SerializationInfo info, StreamingContext context)
		//	: base(info, context)
		//{
		//	_paramName = info.GetString("ParamName");
		//}

		//public override void GetObjectData(SerializationInfo info, StreamingContext context)
		//{
		//	base.GetObjectData(info, context);
		//	info.AddValue("ParamName", _paramName, typeof(string));
		//}

		public override string Message
		{
			get
			{
				SetMessageField();

				string s = base.Message;
				if (!string.IsNullOrEmpty(_paramName))
				{
					s += " " + SR.Format(SR.Arg_ParamName_Name, _paramName);
				}

				return s;
			}
		}

		private void SetMessageField()
		{
			if (message == null)
			{
				message = SR.Arg_ArgumentException;
			}
		}

		public virtual string? ParamName => _paramName;

		/// <summary>Throws an exception if <paramref name="argument"/> is null or empty.</summary>
		/// <param name="argument">The string argument to validate as non-null and non-empty.</param>
		/// <param name="paramName">The name of the parameter with which <paramref name="argument"/> corresponds.</param>
		/// <exception cref="ArgumentNullException"><paramref name="argument"/> is null.</exception>
		/// <exception cref="ArgumentException"><paramref name="argument"/> is empty.</exception>
		public static void ThrowIfNullOrEmpty([NotNull] string? argument, [CallerArgumentExpression("argument")] string? paramName = null)
		{
			if (string.IsNullOrEmpty(argument))
			{
				ThrowNullOrEmptyException(argument, paramName);
			}
		}

		[DoesNotReturn]
		private static void ThrowNullOrEmptyException(string? argument, string? paramName)
		{
			ArgumentNullException.ThrowIfNull(argument, paramName);
			throw new ArgumentException(SR.Argument_EmptyString, paramName);
		}
	}
}
