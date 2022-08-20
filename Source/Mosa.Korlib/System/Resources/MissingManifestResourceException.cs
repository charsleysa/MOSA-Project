// Copyright (c) MOSA Project. Licensed under the New BSD License.
#nullable enable

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.Serialization;

namespace System.Resources
{
	[Serializable]
	public class MissingManifestResourceException : SystemException
	{
		public MissingManifestResourceException()
			: base(SR.Arg_MissingManifestResourceException)
		{
		}

		public MissingManifestResourceException(string? message)
			: base(message)
		{
		}

		public MissingManifestResourceException(string? message, Exception? inner)
			: base(message, inner)
		{
		}

		protected MissingManifestResourceException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{ }
	}
}
