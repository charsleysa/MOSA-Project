﻿// Copyright (c) MOSA Project. Licensed under the New BSD License.

using Mosa.DeviceSystem;
using System.Collections.Generic;

namespace Mosa.DeviceDriver
{
	public static class Setup
	{
		public static List<DeviceDriverRegistryEntry> GetDeviceDriverRegistryEntries()
		{
			return new List<DeviceDriverRegistryEntry>()
			{
				new ISADeviceDriverRegistryEntry()
				{
					Name = "CMOS",
					Platforms = PlatformArchitecture.X86,
					AutoLoad = true,
					BasePort = 0x0070,
					PortRange = 2,
					Factory = delegate { return new ISA.CMOS(); }
				},

				new ISADeviceDriverRegistryEntry()
				{
					Name = "StandardKeyboard",
					Platforms = PlatformArchitecture.X86AndX64,
					AutoLoad = true,
					BasePort = 0x60,
					PortRange = 1,
					AltBasePort = 0x64,
					AltPortRange = 1,
					IRQ = 1,
					Factory = delegate { return new ISA.StandardKeyboard(); }
				},

				new ISADeviceDriverRegistryEntry()
				{
					Name = "PCIController",
					Platforms = PlatformArchitecture.X86AndX64,
					AutoLoad = true,
					BasePort = 0x0CF8,
					PortRange = 8,
					Factory = delegate { return new ISA.PCIController(); }
				},

				new ISADeviceDriverRegistryEntry()
				{
					Name = "IDEController",
					Platforms = PlatformArchitecture.X86AndX64,
					AutoLoad = true,
					BasePort = 0x1F0,
					PortRange = 8,
					AltBasePort = 0x3F6,
					AltPortRange = 8,
					Factory = delegate { return new ISA.IDEController(); }
				},

				new ISADeviceDriverRegistryEntry()
				{
					Name = "IDEController (Secondary)",
					Platforms = PlatformArchitecture.X86AndX64,
					AutoLoad = true,
					BasePort = 0x170,
					PortRange = 8,
					AltBasePort = 0x376,
					AltPortRange = 8,
					ForceOption = "ide2",
					Factory = delegate { return new ISA.IDEController(); }
				},

				new PCIDeviceDriverRegistryEntry()
				{
					Name = "VMwareSGAII",
					Platforms = PlatformArchitecture.X86AndX64,
					VendorID = 0x15AD,
					DeviceID = 0x0405,
					PCIFields = PCIField.VendorID | PCIField.DeviceID,
					Factory = delegate { return new PCI.VMware.VMwareSVGAII(); }
				},

				new PCIDeviceDriverRegistryEntry()
				{
					Name = "GenericVGA",
					Platforms = PlatformArchitecture.X86AndX64,
					ClassCode = 0X03,
					SubClassCode = 0x00,
					ProgIF = 0x00,
					PCIFields = PCIField.ClassCode | PCIField.SubClassCode | PCIField.ProgIF,
					Factory = delegate { return new PCI.VideoCard.GenericVGA(); }
				},

				new PCIDeviceDriverRegistryEntry()
				{
					Name = "VirtIOBlock",
					Platforms = PlatformArchitecture.X86AndX64,
					VendorID = 0x1AF4,
					DeviceID = 0x1001,
					PCIFields = PCIField.VendorID | PCIField.DeviceID,
					Factory = delegate { return new PCI.VirtIO.VirtIOBlock(); }
				}
			};
		}
	}
}
