// Copyright (c) MOSA Project. Licensed under the New BSD License.
#nullable enable

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System
{
	// The base class for all event classes.
	[Serializable]
	public class EventArgs
	{
		public static readonly EventArgs Empty = new EventArgs();

		public EventArgs()
		{
		}
	}
}
