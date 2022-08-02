// Copyright (c) MOSA Project. Licensed under the New BSD License.

namespace System.Runtime.Serialization
{
	public interface ISerializable
	{
		void GetObjectData(SerializationInfo info, StreamingContext context);
	}
}
