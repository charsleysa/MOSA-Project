// Copyright (c) MOSA Project. Licensed under the New BSD License.
#nullable enable

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace System
{
	/// <summary>
	/// The ArgumentException is thrown when an argument is null when it shouldn't be.
	/// </summary>
	[Serializable]
	public class ArgumentNullException : ArgumentException
	{
		/// <summary>
		/// Creates a new ArgumentNullException with its message string set to a default message explaining an argument was null.
		/// </summary>
		public ArgumentNullException()
			 : base(SR.ArgumentNull_Generic)
		{
			HResult = HResults.E_POINTER;
		}

		public ArgumentNullException(string? paramName)
			: base(SR.ArgumentNull_Generic, paramName)
		{
			HResult = HResults.E_POINTER;
		}

		public ArgumentNullException(string? message, Exception? innerException)
			: base(message, innerException)
		{
			HResult = HResults.E_POINTER;
		}

		public ArgumentNullException(string? paramName, string? message)
			: base(message, paramName)
		{
			HResult = HResults.E_POINTER;
		}

		protected ArgumentNullException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{ }

		/// <summary>Throws an <see cref="ArgumentNullException"/> if <paramref name="argument"/> is null.</summary>
		/// <param name="argument">The reference type argument to validate as non-null.</param>
		/// <param name="paramName">The name of the parameter with which <paramref name="argument"/> corresponds.</param>
		public static void ThrowIfNull([NotNull] object? argument, [CallerArgumentExpression("argument")] string? paramName = null)
		{
			if (argument is null)
			{
				Throw(paramName);
			}
		}

		/// <summary>Throws an <see cref="ArgumentNullException"/> if <paramref name="argument"/> is null.</summary>
		/// <param name="argument">The pointer argument to validate as non-null.</param>
		/// <param name="paramName">The name of the parameter with which <paramref name="argument"/> corresponds.</param>
		[CLSCompliant(false)]
		public static unsafe void ThrowIfNull([NotNull] void* argument, [CallerArgumentExpression("argument")] string? paramName = null)
		{
			if (argument is null)
			{
				Throw(paramName);
			}
		}

		/// <summary>Throws an <see cref="ArgumentNullException"/> if <paramref name="argument"/> is null.</summary>
		/// <param name="argument">The pointer argument to validate as non-null.</param>
		/// <param name="paramName">The name of the parameter with which <paramref name="argument"/> corresponds.</param>
		internal static unsafe void ThrowIfNull(IntPtr argument, [CallerArgumentExpression("argument")] string? paramName = null)
		{
			if (argument == IntPtr.Zero)
			{
				Throw(paramName);
			}
		}

		[DoesNotReturn]
		internal static void Throw(string? paramName) =>
			throw new ArgumentNullException(paramName);
	}
}
