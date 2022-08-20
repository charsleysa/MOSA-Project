// Copyright (c) MOSA Project. Licensed under the New BSD License.
#nullable enable

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.Serialization;

namespace System
{
	/// <summary>
	/// The ApplicationException is the base class for nonfatal,
	/// application errors that occur.  These exceptions are generated
	/// (i.e., thrown) by an application, not the Runtime. Applications that need
	/// to create their own exceptions do so by extending this class.
	/// ApplicationException extends but adds no new functionality to
	/// RecoverableException.
	/// </summary>
	[Serializable]
	public class ApplicationException : Exception
	{
		/// <summary>
		/// Creates a new ApplicationException with its message string set to the empty string, its HRESULT set to COR_E_APPLICATION, and its ExceptionInfo reference set to null.
		/// </summary>
		public ApplicationException()
			: base(SR.Arg_ApplicationException)
		{
			HResult = HResults.COR_E_APPLICATION;
		}

		/// <summary>
		/// Creates a new ApplicationException with its message string set to message, its HRESULT set to COR_E_APPLICATION, and its ExceptionInfo reference set to null.
		/// </summary>
		/// <param name="message">The message string</param>
		public ApplicationException(string? message)
			: base(message)
		{
			HResult = HResults.COR_E_APPLICATION;
		}

		public ApplicationException(string? message, Exception? innerException)
			: base(message, innerException)
		{
			HResult = HResults.COR_E_APPLICATION;
		}

		protected ApplicationException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{ }
	}
}
