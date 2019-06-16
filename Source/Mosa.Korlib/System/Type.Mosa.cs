// Copyright (c) MOSA Project. Licensed under the New BSD License.

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Reflection;
using System.Runtime.CompilerServices;
using StackCrawlMark = System.Threading.StackCrawlMark;

namespace System
{
	public abstract partial class Type : MemberInfo, IReflect
	{
		public bool IsInterface
		{
			get
			{
				if (this is RuntimeType rt)
					return RuntimeTypeHandle.IsInterface(rt);
				return ((GetAttributeFlagsImpl() & TypeAttributes.ClassSemanticsMask) == TypeAttributes.Interface);
			}
		}

		[System.Security.DynamicSecurityMethod] // Methods containing StackCrawlMark local var has to be marked DynamicSecurityMethod
		public static Type? GetType(string typeName, bool throwOnError, bool ignoreCase)
		{
			StackCrawlMark stackMark = StackCrawlMark.LookForMyCaller;
			return RuntimeType.GetType(typeName, throwOnError, ignoreCase, ref stackMark);
		}

		[System.Security.DynamicSecurityMethod] // Methods containing StackCrawlMark local var has to be marked DynamicSecurityMethod
		public static Type? GetType(string typeName, bool throwOnError)
		{
			StackCrawlMark stackMark = StackCrawlMark.LookForMyCaller;
			return RuntimeType.GetType(typeName, throwOnError, false, ref stackMark);
		}

		[System.Security.DynamicSecurityMethod] // Methods containing StackCrawlMark local var has to be marked DynamicSecurityMethod
		public static Type? GetType(string typeName)
		{
			StackCrawlMark stackMark = StackCrawlMark.LookForMyCaller;
			return RuntimeType.GetType(typeName, false, false, ref stackMark);
		}

		[System.Security.DynamicSecurityMethod] // Methods containing StackCrawlMark local var has to be marked DynamicSecurityMethod
		public static Type? GetType(
			string typeName,
			Func<AssemblyName, Assembly?>? assemblyResolver,
			Func<Assembly?, string, bool, Type?>? typeResolver)
		{
			StackCrawlMark stackMark = StackCrawlMark.LookForMyCaller;
			return RuntimeType.GetType(typeName, assemblyResolver, typeResolver, false, false, ref stackMark);
		}

		[System.Security.DynamicSecurityMethod] // Methods containing StackCrawlMark local var has to be marked DynamicSecurityMethod
		public static Type? GetType(
			string typeName,
			Func<AssemblyName, Assembly?>? assemblyResolver,
			Func<Assembly?, string, bool, Type?>? typeResolver,
			bool throwOnError)
		{
			StackCrawlMark stackMark = StackCrawlMark.LookForMyCaller;
			return RuntimeType.GetType(typeName, assemblyResolver, typeResolver, throwOnError, false, ref stackMark);
		}

		[System.Security.DynamicSecurityMethod] // Methods containing StackCrawlMark local var has to be marked DynamicSecurityMethod
		public static Type? GetType(
			string typeName,
			Func<AssemblyName, Assembly?>? assemblyResolver,
			Func<Assembly?, string, bool, Type?>? typeResolver,
			bool throwOnError,
			bool ignoreCase)
		{
			StackCrawlMark stackMark = StackCrawlMark.LookForMyCaller;
			return RuntimeType.GetType(typeName, assemblyResolver, typeResolver, throwOnError, ignoreCase, ref stackMark);
		}

		public static Type GetTypeFromProgID(string progID, string? server, bool throwOnError)
		{
			throw new PlatformNotSupportedException();
		}

		public static Type GetTypeFromCLSID(Guid clsid, string? server, bool throwOnError)
		{
			throw new PlatformNotSupportedException();
		}

		internal virtual RuntimeTypeHandle GetTypeHandleInternal()
		{
			return TypeHandle;
		}

		// Given a class handle, this will return the class for that handle.
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern RuntimeType GetTypeFromHandleUnsafe(IntPtr handle);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern Type? GetTypeFromHandle(RuntimeTypeHandle handle);

		// This is only ever called on RuntimeType objects.
		internal virtual string FormatTypeName()
		{
			throw new NotImplementedException();
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool operator ==(Type? left, Type? right);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool operator !=(Type? left, Type? right);

		// Exists to faciliate code sharing between CoreCLR and CoreRT.
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal bool IsRuntimeImplemented() => this is RuntimeType;
	}
}
