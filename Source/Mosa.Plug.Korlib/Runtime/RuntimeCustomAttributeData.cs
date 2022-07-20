// Copyright (c) MOSA Project. Licensed under the New BSD License.

using Mosa.Runtime;
using Mosa.Runtime.Metadata;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using Pointer = Mosa.Runtime.Pointer;

namespace Mosa.Plug.Korlib.Runtime
{
	public sealed unsafe class RuntimeCustomAttributeData : CustomAttributeData
	{
		private Type attributeType;
		private List<CustomAttributeTypedArgument> ctorArgs = new();
		private List<CustomAttributeNamedArgument> namedArgs = new();

		// We use this for cheats
		internal readonly TypeDefinition EnumTypePtr;

		public override ConstructorInfo Constructor => base.Constructor;
		public override IList<CustomAttributeTypedArgument> ConstructorArguments => ctorArgs;
		public override IList<CustomAttributeNamedArgument> NamedArguments => namedArgs;

		public RuntimeCustomAttributeData(CustomAttribute customAttributeTable)
		{
			var attributeTypeDefinition = customAttributeTable.AttributeType;
			var typeHandle = Unsafe.As<TypeDefinition, RuntimeTypeHandle>(ref attributeTypeDefinition);

			attributeType = Type.GetTypeFromHandle(typeHandle);

			// Get the metadata pointer for the enum type
			typeHandle = typeof(Enum).TypeHandle;

			EnumTypePtr = new TypeDefinition(new Pointer(typeHandle.Value));

			for (uint i = 0; i < customAttributeTable.NumberOfArguments; i++)
			{
				// Get the argument metadata pointer
				var argument = customAttributeTable.GetCustomAttributeArgument(i);

				// Get the argument name (if any)
				string name = argument.Name;

				// Get the argument type
				var argTypeDefinition = argument.ArgumentType;
				var argTypeHandle = Unsafe.As<TypeDefinition, RuntimeTypeHandle>(ref argTypeDefinition);

				var argType = Type.GetTypeFromHandle(argTypeHandle);

				// Get the argument value
				var value = ResolveArgumentValue(argument, argType);

				// If the argument has a name then its a NamedArgument, otherwise its a TypedArgument
				if (name is null)
				{
					ctorArgs.Add(CreateTypedArgumentStruct(argType, value));
				}
				else
				{
					var memberInfo = attributeType.GetMember(name)[0];
					namedArgs.Add(CreateNamedArgumentStruct(memberInfo, argType, value, argument.IsField));
				}
			}
		}

		public CustomAttributeTypedArgument CreateTypedArgumentStruct(Type type, object value)
		{
			return new CustomAttributeTypedArgument(type, value);
		}

		public CustomAttributeNamedArgument CreateNamedArgumentStruct(MemberInfo memberInfo, Type type, object value, bool isField)
		{
			var typeArgument = new CustomAttributeTypedArgument(type, value);
			var namedArgument = new CustomAttributeNamedArgument(memberInfo, typeArgument);

			return namedArgument;
		}

		private object ResolveArgumentValue(CustomAttributeArgument argument, Type type)
		{
			var typeCode = argument.ArgumentType.TypeCode;
			var valuePtr = argument.GetArgumentValue();

			// If its an enum type
			if (argument.ArgumentType.ParentType.Handle == EnumTypePtr.Handle)
			{
				typeCode = argument.ArgumentType.ElementType.TypeCode;
			}

			switch (typeCode)
			{
				// 1 byte
				case TypeElementCode.Boolean:
					return valuePtr.Load8() != 0;

				case TypeElementCode.U1:
					return valuePtr.Load8();

				case TypeElementCode.I1:
					return (sbyte)valuePtr.Load8();

				// 2 bytes
				case TypeElementCode.Char:
					return (char)valuePtr.Load16();

				case TypeElementCode.U2:
					return valuePtr.Load16();

				case TypeElementCode.I2:
					return (short)valuePtr.Load16();

				// 4 bytes
				case TypeElementCode.U4:
					return valuePtr.Load32();

				case TypeElementCode.I4:
					return (int)valuePtr.Load32();

				case TypeElementCode.R4:
					return valuePtr.LoadR4();

				// 8 bytes
				case TypeElementCode.U8:
					return valuePtr.Load64();

				case TypeElementCode.I8:
					return (long)valuePtr.Load64();

				case TypeElementCode.R8:
					return valuePtr.LoadR8();

				// SZArray
				case TypeElementCode.SZArray:
					return ResolveArrayValue(argument, type);

				// String
				case TypeElementCode.String:
					return (string)Intrinsic.GetObjectFromAddress(valuePtr);

				default:
					if (type.FullName == "System.Type")
					{
						// Get the argument type
						var argTypeDefinition = argument.ArgumentType;
						var argTypeHandle = Unsafe.As<TypeDefinition, RuntimeTypeHandle>(ref argTypeDefinition);

						return Type.GetTypeFromHandle(argTypeHandle);
					}
					throw new ArgumentException();
			}
		}

		private object ResolveArrayValue(CustomAttributeArgument argument, Type type)
		{
			var typeCode = argument.ArgumentType.ElementType.TypeCode;
			var valuePtr = argument.GetArgumentValue();
			var size = ((uint*)valuePtr)[0];
			valuePtr += IntPtr.Size;

			switch (typeCode)
			{
				// 1 byte
				case TypeElementCode.Boolean:
					{
						bool[] array = new bool[size];
						for (int i = 0; i < size; i++)
							array[i] = ((bool*)valuePtr)[i];
						return array;
					}

				case TypeElementCode.U1:
					{
						byte[] array = new byte[size];
						for (int i = 0; i < size; i++)
							array[i] = ((byte*)valuePtr)[i];
						return array;
					}

				case TypeElementCode.I1:
					{
						sbyte[] array = new sbyte[size];
						for (int i = 0; i < size; i++)
							array[i] = ((sbyte*)valuePtr)[i];
						return array;
					}

				// 2 bytes
				case TypeElementCode.Char:
					{
						char[] array = new char[size];
						for (int i = 0; i < size; i++)
							array[i] = ((char*)valuePtr)[i];
						return array;
					}

				case TypeElementCode.U2:
					{
						ushort[] array = new ushort[size];
						for (int i = 0; i < size; i++)
							array[i] = ((ushort*)valuePtr)[i];
						return array;
					}

				case TypeElementCode.I2:
					{
						short[] array = new short[size];
						for (int i = 0; i < size; i++)
							array[i] = ((short*)valuePtr)[i];
						return array;
					}

				// 4 bytes
				case TypeElementCode.U4:
					{
						uint[] array = new uint[size];
						for (int i = 0; i < size; i++)
							array[i] = ((uint*)valuePtr)[i];
						return array;
					}

				case TypeElementCode.I4:
					{
						int[] array = new int[size];
						for (int i = 0; i < size; i++)
							array[i] = ((int*)valuePtr)[i];
						return array;
					}

				case TypeElementCode.R4:
					{
						float[] array = new float[size];
						for (int i = 0; i < size; i++)
							array[i] = ((float*)valuePtr)[i];
						return array;
					}

				// 8 bytes
				case TypeElementCode.U8:
					{
						ulong[] array = new ulong[size];
						for (int i = 0; i < size; i++)
							array[i] = ((ulong*)valuePtr)[i];
						return array;
					}

				case TypeElementCode.I8:
					{
						long[] array = new long[size];
						for (int i = 0; i < size; i++)
							array[i] = ((long*)valuePtr)[i];
						return array;
					}

				case TypeElementCode.R8:
					{
						double[] array = new double[size];
						for (int i = 0; i < size; i++)
							array[i] = ((double*)valuePtr)[i];
						return array;
					}

				default:

					// TODO: Enums
					// What kind of array is this!?
					throw new NotSupportedException();
			}
		}
	}
}
