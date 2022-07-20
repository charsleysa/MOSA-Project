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
	public sealed unsafe class RuntimeAssembly : Assembly
	{
		internal AssemblyDefinition assemblyDefinition;
		internal readonly List<RuntimeTypeInfo> typeInfoList;
		internal readonly List<RuntimeTypeHandle> typeHandles;
		internal List<CustomAttributeData> customAttributesData = null;

		private readonly string fullName;

		public override IEnumerable<CustomAttributeData> CustomAttributes
		{
			get
			{
				if (customAttributesData == null)
				{
					// Custom Attributes Data - Lazy load
					// FIXME: Race condition
					if (!assemblyDefinition.CustomAttributes.IsNull)
					{
						var customAttributesTablePtr = assemblyDefinition.CustomAttributes;
						var customAttributesCount = customAttributesTablePtr.NumberOfAttributes;
						customAttributesData = new List<CustomAttributeData>();
						for (uint i = 0; i < customAttributesCount; i++)
						{
							var cad = new RuntimeCustomAttributeData(customAttributesTablePtr.GetCustomAttribute(i));
							customAttributesData.Add(cad);
						}
					}
				}

				return customAttributesData;
			}
		}

		public override IEnumerable<TypeInfo> DefinedTypes
		{
			get
			{
				var types = new List<TypeInfo>();

				foreach (var type in typeInfoList)
					types.Add(type);

				return types;
			}
		}

		public override string FullName
		{
			get { return fullName; }
		}

		public override IEnumerable<Type> ExportedTypes
		{
			get
			{
				var list = new List<RuntimeTypeInfo>();
				foreach (var type in typeInfoList)
				{
					if ((type.Attributes & TypeAttributes.VisibilityMask) != TypeAttributes.Public)
						continue;
					list.Add(type);
				}
				return list;
			}
		}

		internal RuntimeAssembly(IntPtr pointer)
		{
			assemblyDefinition = new AssemblyDefinition(new Pointer(pointer));
			fullName = assemblyDefinition.Name;

			typeHandles = new List<RuntimeTypeHandle>();
			typeInfoList = new List<RuntimeTypeInfo>();

			var typeCount = assemblyDefinition.NumberOfTypes;

			for (uint i = 0; i < typeCount; i++)
			{
				var typeDefinition = assemblyDefinition.GetTypeDefinition(i);
				var handle = Unsafe.As<TypeDefinition, RuntimeTypeHandle>(ref typeDefinition);

				if (typeHandles.Contains(handle))
					continue;

				typeHandles.Add(handle);

				var type = new RuntimeTypeInfo(handle, this);

				typeInfoList.Add(type);
			}
		}
	}
}
