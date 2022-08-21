// Copyright (c) MOSA Project. Licensed under the New BSD License.
#nullable enable

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Reflection;

namespace System.Runtime.Loader
{
	public sealed class AssemblyDependencyResolver
	{
		public AssemblyDependencyResolver(string componentAssemblyPath)
		{
			throw new PlatformNotSupportedException();
		}

		public string? ResolveAssemblyToPath(AssemblyName assemblyName)
		{
			throw new PlatformNotSupportedException();
		}

		public string? ResolveUnmanagedDllToPath(string unmanagedDllName)
		{
			throw new PlatformNotSupportedException();
		}
	}
}
