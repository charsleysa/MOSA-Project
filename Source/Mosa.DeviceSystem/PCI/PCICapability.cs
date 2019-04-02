// Copyright (c) MOSA Project. Licensed under the New BSD License.

namespace Mosa.DeviceSystem.PCI
{
	public struct PCICapabilityID
	{
		/// <summary>
		/// Power Management
		/// </summary>
		public const byte PM = 0x01;

		/// <summary>
		/// Accelerated Graphics Port
		/// </summary>
		public const byte AGP = 0x02;

		/// <summary>
		/// Vital Product Data
		/// </summary>
		public const byte VPD = 0x03;

		/// <summary>
		/// Slot Identification
		/// </summary>
		public const byte SLOTID = 0x04;

		/// <summary>
		/// Message Signalled Interrupts
		/// </summary>
		public const byte MSI = 0x05;

		/// <summary>
		/// CompactPCI HotSwap
		/// </summary>
		public const byte CHSWP = 0x06;

		/// <summary>
		/// PCI-X
		/// </summary>
		public const byte PCIX = 0x07;

		/// <summary>
		/// HyperTransport
		/// </summary>
		public const byte HT = 0x08;

		/// <summary>
		/// Vendor-Specific
		/// </summary>
		public const byte VNDR = 0x09;

		/// <summary>
		/// Debug port
		/// </summary>
		public const byte DBG = 0x0A;

		/// <summary>
		/// CompactPCI Central Resource Control
		/// </summary>
		public const byte CCRC = 0x0B;

		/// <summary>
		/// PCI Standard Hot-Plug Controller
		/// </summary>
		public const byte SHPC = 0x0C;

		/// <summary>
		/// Bridge subsystem vendor/device ID
		/// </summary>
		public const byte SSVID = 0x0D;

		/// <summary>
		/// AGP Target PCI-PCI bridge
		/// </summary>
		public const byte AGP3 = 0x0E;

		/// <summary>
		/// Secure Device
		/// </summary>
		public const byte SECDEV = 0x0F;

		/// <summary>
		/// PCI Express
		/// </summary>
		public const byte EXP = 0x10;

		/// <summary>
		/// MSI-X
		/// </summary>
		public const byte MSIX = 0x11;

		/// <summary>
		/// SATA Data/Index Conf.
		/// </summary>
		public const byte SATA = 0x12;

		/// <summary>
		/// PCI Advanced Features
		/// </summary>
		public const byte AF = 0x13;
	}

	public struct PCICapability
	{
		public byte register;
		public byte capabilityID;
	}
}
