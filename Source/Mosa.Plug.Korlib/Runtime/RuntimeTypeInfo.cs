// Copyright (c) MOSA Project. Licensed under the New BSD License.

using Mosa.Runtime;
using Mosa.Runtime.Metadata;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Reflection;
using System.Runtime.CompilerServices;
using Pointer = Mosa.Runtime.Pointer;

namespace Mosa.Plug.Korlib.Runtime
{
	public sealed unsafe class RuntimeTypeInfo : TypeInfo
	{
		private readonly TypeDefinition typeDefinition;
		private readonly TypeElementCode typeCode;
		private readonly Type baseType;
		private readonly Type elementType;
		private List<CustomAttributeData> customAttributesData = null;

		internal readonly Type ValueType = typeof(ValueType);
		internal readonly Type EnumType = typeof(Enum);

		public override string AssemblyQualifiedName { get; }

		public override Assembly Assembly { get; }

		public override Type BaseType
		{ get { return (IsInterface) ? null : baseType; } }

		public override bool ContainsGenericParameters => throw new NotImplementedException();

		public override IEnumerable<CustomAttributeData> CustomAttributes
		{
			get
			{
				if (customAttributesData == null)
				{
					// Custom Attributes Data - Lazy load
					customAttributesData = new List<CustomAttributeData>();
					if (!typeDefinition.CustomAttributes.IsNull)
					{
						var customAttributesTable = typeDefinition.CustomAttributes;
						var customAttributesCount = customAttributesTable.NumberOfAttributes;
						for (uint i = 0; i < customAttributesCount; i++)
						{
							RuntimeCustomAttributeData cad = new RuntimeCustomAttributeData(customAttributesTable.GetCustomAttribute(i));
							customAttributesData.Add(cad);
						}
					}
				}

				return customAttributesData;
			}
		}

		public override MethodBase DeclaringMethod => throw new NotImplementedException();

		public override Type DeclaringType { get; }

		public override string FullName { get; }

		public override int GenericParameterPosition
		{
			get { throw new NotSupportedException(); }
		}

		public override Type[] GenericTypeArguments
		{
			get { return new Type[0]; }
		}

		public override bool IsEnum
		{
			get { return BaseType == EnumType; }
		}

		public override bool IsGenericParameter
		{
			// We don't know so just return false
			get { return false; }
		}

		public override bool IsGenericType
		{
			// We don't know so just return false
			get { return false; }
		}

		public override bool IsGenericTypeDefinition
		{
			// We don't know so just return false
			get { return false; }
		}

		public override bool IsSerializable
		{
			// We don't know so just return false
			get { return false; }
		}

		public override string Name { get; }

		public override string Namespace { get; }

		public override Guid GUID => throw new NotImplementedException();

		public override Module Module => throw new NotImplementedException();

		public override Type UnderlyingSystemType => throw new NotImplementedException();

		public RuntimeTypeInfo(RuntimeTypeHandle handle, Assembly assembly)
		{
			Assembly = assembly;

			typeDefinition = new TypeDefinition(new Pointer(handle.Value));

			AssemblyQualifiedName = typeDefinition.Name;   // TODO
			Name = typeDefinition.Name;                    // TODO
			Namespace = typeDefinition.Name;              // TODO
			FullName = typeDefinition.Name;

			typeCode = typeDefinition.TypeCode;

			// Base Type
			if (!typeDefinition.ParentType.IsNull)
			{
				var parentTypeDefinition = typeDefinition.ParentType;
				var parentHandle = Unsafe.As<TypeDefinition, RuntimeTypeHandle>(ref parentTypeDefinition);

				baseType = Type.GetTypeFromHandle(parentHandle);
			}

			// Declaring Type
			if (!typeDefinition.DeclaringType.IsNull)
			{
				var declaringTypeDefinition = typeDefinition.DeclaringType;
				var declaringHandle = Unsafe.As<TypeDefinition, RuntimeTypeHandle>(ref declaringTypeDefinition);

				DeclaringType = Type.GetTypeFromHandle(declaringHandle);
			}

			// Element Type
			if (!typeDefinition.ElementType.IsNull)
			{
				var elementTypeDefinition = typeDefinition.ElementType;
				var elementHandle = Unsafe.As<TypeDefinition, RuntimeTypeHandle>(ref elementTypeDefinition);

				elementType = Type.GetTypeFromHandle(elementHandle);
			}
		}

		public override Type AsType()
		{
			return this;
		}

		public override int GetArrayRank()
		{
			// We don't know so just return 1 if array, 0 otherwise
			return IsArrayImpl() ? 1 : 0;
		}

		public override Type GetElementType()
		{
			return elementType;
		}

		public override Type[] GetGenericParameterConstraints()
		{
			// No planned support
			throw new NotSupportedException();
		}

		public override Type GetGenericTypeDefinition()
		{
			// No planned support
			throw new NotSupportedException();
		}

		protected override bool HasElementTypeImpl()
		{
			return elementType != null;
		}

		protected override bool IsArrayImpl()
		{
			return typeCode == TypeElementCode.Array || typeCode == TypeElementCode.SZArray;
		}

		protected override bool IsByRefImpl()
		{
			return typeCode == TypeElementCode.ManagedPointer;
		}

		protected override bool IsPointerImpl()
		{
			return typeCode == TypeElementCode.UnmanagedPointer;
		}

		protected override bool IsPrimitiveImpl()
		{
			return typeCode == TypeElementCode.Boolean
				|| typeCode == TypeElementCode.Char
				|| (typeCode >= TypeElementCode.I && typeCode <= TypeElementCode.I8)
				|| (typeCode >= TypeElementCode.U && typeCode <= TypeElementCode.U8)
				|| typeCode == TypeElementCode.R4
				|| typeCode == TypeElementCode.R8;
		}

		protected override bool IsValueTypeImpl()
		{
			Type thisType = AsType();
			if (thisType == ValueType || thisType == EnumType)
				return false;

			return IsSubclassOf(ValueType);
		}

		public override Type MakeArrayType()
		{
			// No planned support
			throw new NotSupportedException();
		}

		public override Type MakeArrayType(int rank)
		{
			// No planned support
			throw new NotSupportedException();
		}

		public override Type MakeByRefType()
		{
			// No planned support
			throw new NotSupportedException();
		}

		public override Type MakeGenericType(params Type[] typeArguments)
		{
			// No planned support
			throw new NotSupportedException();
		}

		public override Type MakePointerType()
		{
			// No planned support
			throw new NotSupportedException();
		}

		protected override TypeAttributes GetAttributeFlagsImpl()
		{
			return typeDefinition.Attributes;
		}

		protected override ConstructorInfo GetConstructorImpl(BindingFlags bindingAttr, Binder binder, CallingConventions callConvention, Type[] types, ParameterModifier[] modifiers)
		{
			throw new NotImplementedException();
		}

		public override ConstructorInfo[] GetConstructors(BindingFlags bindingAttr)
		{
			throw new NotImplementedException();
		}

		public override EventInfo GetEvent(string name, BindingFlags bindingAttr)
		{
			throw new NotImplementedException();
		}

		public override EventInfo[] GetEvents(BindingFlags bindingAttr)
		{
			throw new NotImplementedException();
		}

		public override FieldInfo GetField(string name, BindingFlags bindingAttr)
		{
			throw new NotImplementedException();
		}

		public override FieldInfo[] GetFields(BindingFlags bindingAttr)
		{
			throw new NotImplementedException();
		}

		[return: DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.Interfaces)]
		public override Type GetInterface(string name, bool ignoreCase)
		{
			throw new NotImplementedException();
		}

		public override Type[] GetInterfaces()
		{
			throw new NotImplementedException();
		}

		public override MemberInfo[] GetMembers(BindingFlags bindingAttr)
		{
			throw new NotImplementedException();
		}

		protected override MethodInfo GetMethodImpl(string name, BindingFlags bindingAttr, Binder binder, CallingConventions callConvention, Type[] types, ParameterModifier[] modifiers)
		{
			throw new NotImplementedException();
		}

		public override MethodInfo[] GetMethods(BindingFlags bindingAttr)
		{
			throw new NotImplementedException();
		}

		public override Type GetNestedType(string name, BindingFlags bindingAttr)
		{
			throw new NotImplementedException();
		}

		public override Type[] GetNestedTypes(BindingFlags bindingAttr)
		{
			throw new NotImplementedException();
		}

		public override PropertyInfo[] GetProperties(BindingFlags bindingAttr)
		{
			throw new NotImplementedException();
		}

		protected override PropertyInfo GetPropertyImpl(string name, BindingFlags bindingAttr, Binder binder, Type returnType, Type[] types, ParameterModifier[] modifiers)
		{
			throw new NotImplementedException();
		}

		public override object InvokeMember(string name, BindingFlags invokeAttr, Binder binder, object target, object[] args, ParameterModifier[] modifiers, CultureInfo culture, string[] namedParameters)
		{
			throw new NotImplementedException();
		}

		protected override bool IsCOMObjectImpl()
		{
			throw new NotImplementedException();
		}

		public override object[] GetCustomAttributes(bool inherit)
		{
			throw new NotImplementedException();
		}

		public override object[] GetCustomAttributes(Type attributeType, bool inherit)
		{
			throw new NotImplementedException();
		}

		public override bool IsDefined(Type attributeType, bool inherit)
		{
			throw new NotImplementedException();
		}
	}
}
