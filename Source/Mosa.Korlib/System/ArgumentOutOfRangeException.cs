// Copyright (c) MOSA Project. Licensed under the New BSD License.
#nullable enable

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.Serialization;

namespace System
{
	/// <summary>
	/// The ArgumentOutOfRangeException is thrown when an argument is outside the legal range for that argument.
	/// </summary>
	[Serializable]
	public class ArgumentOutOfRangeException : ArgumentException
	{
		private readonly object? _actualValue;

		/// <summary>
		/// Creates a new ArgumentOutOfRangeException with its message string set to a default message explaining an argument was out of range.
		/// </summary>
		public ArgumentOutOfRangeException()
			: base(SR.Arg_ArgumentOutOfRangeException)
		{
			HResult = HResults.COR_E_ARGUMENTOUTOFRANGE;
		}

		public ArgumentOutOfRangeException(string? paramName)
			: base(SR.Arg_ArgumentOutOfRangeException, paramName)
		{
			HResult = HResults.COR_E_ARGUMENTOUTOFRANGE;
		}

		public ArgumentOutOfRangeException(string? paramName, string? message)
			: base(message, paramName)
		{
			HResult = HResults.COR_E_ARGUMENTOUTOFRANGE;
		}

		public ArgumentOutOfRangeException(string? message, Exception? innerException)
			: base(message, innerException)
		{
			HResult = HResults.COR_E_ARGUMENTOUTOFRANGE;
		}

		public ArgumentOutOfRangeException(string? paramName, object? actualValue, string? message)
			: base(message, paramName)
		{
			_actualValue = actualValue;
			HResult = HResults.COR_E_ARGUMENTOUTOFRANGE;
		}

		protected ArgumentOutOfRangeException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
			_actualValue = info.GetValue("ActualValue", typeof(object));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("ActualValue", _actualValue, typeof(object));
		}

		public override string Message
		{
			get
			{
				string s = base.Message;
				if (_actualValue != null)
				{
					string valueMessage = SR.Format(SR.ArgumentOutOfRange_ActualValue, _actualValue);
					if (s == null)
						return valueMessage;
					return s + Environment.NewLineConst + valueMessage;
				}
				return s;
			}
		}

		public virtual object? ActualValue => _actualValue;
	}
}
