// Copyright (c) MOSA Project. Licensed under the New BSD License.
#nullable enable

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Runtime.InteropServices
{
	public enum ComInterfaceType
	{
		InterfaceIsDual = 0,
		InterfaceIsIUnknown = 1,
		InterfaceIsIDispatch = 2,
		InterfaceIsIInspectable = 3,
	}
}
