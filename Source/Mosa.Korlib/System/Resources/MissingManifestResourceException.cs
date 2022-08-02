// Copyright (c) MOSA Project. Licensed under the New BSD License.

//using System.Runtime.Serialization;

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

		//protected MissingManifestResourceException(SerializationInfo info, StreamingContext context)
		//	: base(info, context)
		//{
		//}
	}
}
