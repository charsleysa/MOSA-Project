// Copyright (c) MOSA Project. Licensed under the New BSD License.
#nullable enable

//using System.Runtime.Serialization;

namespace System
{
	[Serializable]
	public partial class TypeLoadException : SystemException //, ISerializable
	{
		public TypeLoadException()
			: base(SR.Arg_TypeLoadException)
		{
			HResult = HResults.COR_E_TYPELOAD;
		}

		public TypeLoadException(string? message)
			: base(message)
		{
			HResult = HResults.COR_E_TYPELOAD;
		}

		public TypeLoadException(string? message, Exception? inner)
			: base(message, inner)
		{
			HResult = HResults.COR_E_TYPELOAD;
		}

		// This is called from inside the runtime
		private TypeLoadException(string? className,
			string? assemblyName,
			string? messageArg,
			int resourceId)
			: base(null)
		{
			HResult = HResults.COR_E_TYPELOAD;
			_className = className;
			_assemblyName = assemblyName;
			_messageArg = messageArg;
			_resourceId = resourceId;

			// Set the _message field eagerly; debuggers look at this field to
			// display error info. They don't call the Message property.
			SetMessageField();
		}

		private void SetMessageField()
		{
			if (message == null)
			{
				if (_className == null && _resourceId == 0)
				{
					message = SR.Arg_TypeLoadException;
				}
				else
				{
					// TODO
					_assemblyName ??= SR.IO_UnknownFileName;
					_className ??= SR.IO_UnknownFileName;

					message = SR.Arg_TypeLoadException;
				}
			}
		}

		public override string Message
		{
			get
			{
				SetMessageField();
				return message!;
			}
		}

		public string TypeName => _className ?? string.Empty;

		//protected TypeLoadException(SerializationInfo info, StreamingContext context) : base(info, context)
		//{
		//	_className = info.GetString("TypeLoadClassName");
		//	_assemblyName = info.GetString("TypeLoadAssemblyName");
		//	_messageArg = info.GetString("TypeLoadMessageArg");
		//	_resourceId = info.GetInt32("TypeLoadResourceID");
		//}

		//public override void GetObjectData(SerializationInfo info, StreamingContext context)
		//{
		//	base.GetObjectData(info, context);
		//	info.AddValue("TypeLoadClassName", _className, typeof(string));
		//	info.AddValue("TypeLoadAssemblyName", _assemblyName, typeof(string));
		//	info.AddValue("TypeLoadMessageArg", _messageArg, typeof(string));
		//	info.AddValue("TypeLoadResourceID", _resourceId);
		//}

		private string? _className;
		private string? _assemblyName;
		private readonly string? _messageArg;
		private readonly int _resourceId;
	}
}
