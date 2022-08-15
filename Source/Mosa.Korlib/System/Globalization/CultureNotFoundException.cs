// Copyright (c) MOSA Project. Licensed under the New BSD License.
#nullable enable

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

//using System.Runtime.Serialization;

namespace System.Globalization
{
	[Serializable]
	public class CultureNotFoundException : ArgumentException
	{
		private readonly string? _invalidCultureName; // unrecognized culture name
		private readonly int? _invalidCultureId;     // unrecognized culture Lcid

		public CultureNotFoundException()
			: base(DefaultMessage)
		{
		}

		public CultureNotFoundException(string? message)
			: base(message)
		{
		}

		public CultureNotFoundException(string? paramName, string? message)
			: base(message, paramName)
		{
		}

		public CultureNotFoundException(string? message, Exception? innerException)
			: base(message, innerException)
		{
		}

		public CultureNotFoundException(string? paramName, string? invalidCultureName, string? message)
			: base(message, paramName)
		{
			_invalidCultureName = invalidCultureName;
		}

		public CultureNotFoundException(string? message, string? invalidCultureName, Exception? innerException)
			: base(message, innerException)
		{
			_invalidCultureName = invalidCultureName;
		}

		public CultureNotFoundException(string? message, int invalidCultureId, Exception? innerException)
			: base(message, innerException)
		{
			_invalidCultureId = invalidCultureId;
		}

		public CultureNotFoundException(string? paramName, int invalidCultureId, string? message)
			: base(message, paramName)
		{
			_invalidCultureId = invalidCultureId;
		}

		//protected CultureNotFoundException(SerializationInfo info, StreamingContext context)
		//	: base(info, context)
		//{
		//	_invalidCultureId = (int?)info.GetValue("InvalidCultureId", typeof(int?));
		//	_invalidCultureName = (string?)info.GetValue("InvalidCultureName", typeof(string));
		//}

		//public override void GetObjectData(SerializationInfo info, StreamingContext context)
		//{
		//	base.GetObjectData(info, context);
		//	info.AddValue("InvalidCultureId", _invalidCultureId, typeof(int?));
		//	info.AddValue("InvalidCultureName", _invalidCultureName, typeof(string));
		//}

		public virtual int? InvalidCultureId => _invalidCultureId;

		public virtual string? InvalidCultureName => _invalidCultureName;

		private static string DefaultMessage => SR.Argument_CultureNotSupported;

		private string? FormattedInvalidCultureId =>
			InvalidCultureId != null ?
				string.Format(CultureInfo.InvariantCulture, "{0} (0x{0:x4})", (int)InvalidCultureId) :
				InvalidCultureName;

		public override string Message
		{
			get
			{
				string s = base.Message;
				if (_invalidCultureId != null || _invalidCultureName != null)
				{
					string valueMessage = SR.Format(SR.Argument_CultureInvalidIdentifier, FormattedInvalidCultureId);
					if (s == null)
					{
						return valueMessage;
					}

					return s + Environment.NewLineConst + valueMessage;
				}
				return s;
			}
		}
	}
}
