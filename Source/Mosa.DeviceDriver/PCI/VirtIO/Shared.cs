// Copyright (c) MOSA Project. Licensed under the New BSD License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mosa.DeviceDriver.PCI.VirtIO
{
	unsafe internal struct VirtIOPCICapability
	{
		private byte cap_vndr;
		private byte cap_next;
		private byte cap_len;
		private byte cfg_type;
		private byte bar;
		private byte padding[3];
		private uint offset;
		private uint length;
	}

	internal struct VirtIOPCICapabilityType
	{
		internal const byte CommonConfig = 0x01;
		internal const byte NotifyConfig = 0x02;
		internal const byte ISRConfig = 0x03;
		internal const byte DeviceConfig = 0x04;
		internal const byte PCIConfig = 0x05;
	}

	internal struct VirtIOStatusFlags
	{
		internal const byte DeviceAcknowledged = 0x01;
		internal const byte DriverLoaded = 0x02;
		internal const byte DriverReady = 0x04;
		internal const byte FeaturesAccepted = 0x08;
		internal const byte DeviceError = 0x40;
		internal const byte DriverFailed = 0x80;
	}

	internal struct VirtIORegisters
	{
		internal const byte DeviceFeatures = 0x00;
		internal const byte GuestFeatures = 0x04;
		internal const byte QueueAddress = 0x08;
		internal const byte QueueSize = 0x0C;
		internal const byte QueueSelect = 0x0E;
		internal const byte QueueNotify = 0x10;
		internal const byte DeviceStatus = 0x12;
		internal const byte ISRStatus = 0x13;
	}
}
