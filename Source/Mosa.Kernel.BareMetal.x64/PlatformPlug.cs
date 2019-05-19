﻿// Copyright (c) MOSA Project. Licensed under the New BSD License.

using Mosa.Kernel.BareMetal.BootMemory;
using Mosa.Runtime.Plug;
using System;

namespace Mosa.Kernel.BareMetal.x64
{
	public static class PlatformPlug
	{
		[Plug("Mosa.Kernel.BareMetal.Platform::GetPageShift")]
		public static uint GetPageShift()
		{
			return 12;
		}

		[Plug("Mosa.Kernel.BareMetal.Platform::EntryPoint")]
		public static void EntryPoint()
		{
			// TODO: Get EAX and EBX
			Multiboot.Setup(IntPtr.Zero, 0);

			// TODO: SSE
		}

		[Plug("Mosa.Kernel.BareMetal.Platform::GetMemoryMapAddress")]
		public static IntPtr GetMemoryMapAddress()
		{
			return new IntPtr(0x00007E00);
		}

		[Plug("Mosa.Kernel.BareMetal.Platform::UpdateBootMemoryMap")]
		public static void UpdateBootMemoryMap()
		{
			// Reserve the first 1MB
			BootMemoryMap.SetMemoryMap(new IntPtr(0), 1024 * 1024, BootMemoryMapType.Reserved);
		}

		[Plug("Mosa.Kernel.BareMetal.Platform::GetInitialGCMemoryPool")]
		public static AddressRange GetInitialGCMemoryPool()
		{
			return new AddressRange(0x03000000, 16 * 1024 * 1024); // 16MB @ 48MB
		}
	}
}
