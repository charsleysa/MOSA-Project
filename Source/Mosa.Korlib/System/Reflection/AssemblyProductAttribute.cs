// Copyright (c) MOSA Project. Licensed under the New BSD License.
#nullable enable

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Reflection
{
	[AttributeUsage(AttributeTargets.Assembly, Inherited = false)]
	public sealed class AssemblyProductAttribute : Attribute
	{
		public AssemblyProductAttribute(string product)
		{
			Product = product;
		}

		public string Product { get; }
	}
}
