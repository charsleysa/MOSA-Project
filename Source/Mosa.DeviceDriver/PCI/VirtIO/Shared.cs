// Copyright (c) MOSA Project. Licensed under the New BSD License.

using Mosa.DeviceSystem;

namespace Mosa.DeviceDriver.PCI.VirtIO
{
	internal struct VirtIOPCICapabilityRegisters
	{
		/// <summary>
		/// byte
		/// </summary>
		internal const byte CapabilityID = 0x00;

		/// <summary>
		/// byte
		/// </summary>
		internal const byte NextCapabilityRegister = 0x01;

		/// <summary>
		/// byte
		/// </summary>
		internal const byte CapabilityLength = 0x02;

		/// <summary>
		/// byte
		/// </summary>
		internal const byte ConfigType = 0x03;

		/// <summary>
		/// byte
		/// </summary>
		internal const byte BAR = 0x04;

		/// <summary>
		/// uint
		/// </summary>
		internal const byte Offset = 0x08;

		/// <summary>
		/// uint
		/// </summary>
		internal const byte Length = 0x0C;
	}

	internal struct VirtIOPCICommonConfigRegisters
	{
		internal const byte DeviceFeatureSelect = 0x00;
		internal const byte DeviceFeature = 0x04;
		internal const byte DriverFeatureSelect = 0x08;
		internal const byte DriverFeature = 0x0C;
		internal const byte MSIXConfig = 0x10;
		internal const byte NumberOfQueues = 0x12;
		internal const byte DeviceStatus = 0x14;
		internal const byte ConfigGeneration = 0x15;
		internal const byte QueueSelect = 0x16;
		internal const byte QueueSize = 0x18;
		internal const byte QueueMSIXVector = 0x1A;
		internal const byte QueueEnable = 0x1C;
		internal const byte QueueNotifyOff = 0x1E;
		internal const byte QueueDescriptorTableAddressLow = 0x20;
		internal const byte QueueDescriptorTableAddressHigh = 0x24;
		internal const byte QueueAvailableRingAddressLow = 0x28;
		internal const byte QueueAvailableRingAddressHigh = 0x2C;
		internal const byte QueueUsedRingAddressLow = 0x30;
		internal const byte QueueUsedRingAddressHigh = 0x34;
	}

	internal struct VirtIOPCICommonConfig
	{
		private IOPortReadWriteRange portRange;

		public VirtIOPCICommonConfig(IOPortReadWriteRange portRange)
		{
			this.portRange = portRange;
		}

		internal uint DeviceFeatureSelect
		{
			get { return portRange.Read32(VirtIOPCICommonConfigRegisters.DeviceFeatureSelect); }
			set { portRange.Write32(VirtIOPCICommonConfigRegisters.DeviceFeatureSelect, value); }
		}

		internal uint DeviceFeature
		{
			get { return portRange.Read32(VirtIOPCICommonConfigRegisters.DeviceFeature); }
		}

		internal uint DriverFeatureSelect
		{
			get { return portRange.Read32(VirtIOPCICommonConfigRegisters.DriverFeatureSelect); }
			set { portRange.Write32(VirtIOPCICommonConfigRegisters.DriverFeatureSelect, value); }
		}

		internal uint DriverFeature
		{
			get { return portRange.Read32(VirtIOPCICommonConfigRegisters.DriverFeature); }
			set { portRange.Write32(VirtIOPCICommonConfigRegisters.DriverFeature, value); }
		}

		internal ushort MSIXConfig
		{
			get { return portRange.Read16(VirtIOPCICommonConfigRegisters.MSIXConfig); }
			set { portRange.Write16(VirtIOPCICommonConfigRegisters.MSIXConfig, value); }
		}

		internal ushort NumberOfQueues
		{
			get { return portRange.Read16(VirtIOPCICommonConfigRegisters.NumberOfQueues); }
		}

		internal byte DeviceStatus
		{
			get { return portRange.Read8(VirtIOPCICommonConfigRegisters.DeviceStatus); }
			set { portRange.Write8(VirtIOPCICommonConfigRegisters.DeviceStatus, value); }
		}

		internal byte ConfigGeneration
		{
			get { return portRange.Read8(VirtIOPCICommonConfigRegisters.ConfigGeneration); }
		}

		/* Below are regarding specific virtqueue depending on value in select */

		internal ushort QueueSelect
		{
			get { return portRange.Read16(VirtIOPCICommonConfigRegisters.QueueSelect); }
			set { portRange.Write16(VirtIOPCICommonConfigRegisters.QueueSelect, value); }
		}

		internal ushort QueueSize
		{
			get { return portRange.Read16(VirtIOPCICommonConfigRegisters.QueueSize); }
			set { portRange.Write16(VirtIOPCICommonConfigRegisters.QueueSize, value); }
		}

		internal ushort QueueMSIXVector
		{
			get { return portRange.Read16(VirtIOPCICommonConfigRegisters.QueueMSIXVector); }
			set { portRange.Write16(VirtIOPCICommonConfigRegisters.QueueMSIXVector, value); }
		}

		internal ushort QueueEnable
		{
			get { return portRange.Read16(VirtIOPCICommonConfigRegisters.QueueEnable); }
			set { portRange.Write16(VirtIOPCICommonConfigRegisters.QueueEnable, value); }
		}

		internal ushort QueueNotifyOff
		{
			get { return portRange.Read16(VirtIOPCICommonConfigRegisters.QueueNotifyOff); }
		}

		internal ulong QueueDescriptorTableAddress
		{
			get
			{
				var low = portRange.Read32(VirtIOPCICommonConfigRegisters.QueueDescriptorTableAddressLow);
				var high = portRange.Read32(VirtIOPCICommonConfigRegisters.QueueDescriptorTableAddressHigh);
				return ((ulong)high << 32) | low;
			}
			set
			{
				var low = (uint)(value & 0xFFFFFFFF);
				var high = (uint)(value >> 32);
				portRange.Write32(VirtIOPCICommonConfigRegisters.QueueDescriptorTableAddressLow, low);
				portRange.Write32(VirtIOPCICommonConfigRegisters.QueueDescriptorTableAddressHigh, high);
			}
		}

		internal ulong QueueAvailableRingAddress
		{
			get
			{
				var low = portRange.Read32(VirtIOPCICommonConfigRegisters.QueueAvailableRingAddressLow);
				var high = portRange.Read32(VirtIOPCICommonConfigRegisters.QueueAvailableRingAddressHigh);
				return ((ulong)high << 32) | low;
			}
			set
			{
				var low = (uint)(value & 0xFFFFFFFF);
				var high = (uint)(value >> 32);
				portRange.Write32(VirtIOPCICommonConfigRegisters.QueueAvailableRingAddressLow, low);
				portRange.Write32(VirtIOPCICommonConfigRegisters.QueueAvailableRingAddressHigh, high);
			}
		}

		internal ulong QueueUsedRingAddress
		{
			get
			{
				var low = portRange.Read32(VirtIOPCICommonConfigRegisters.QueueUsedRingAddressLow);
				var high = portRange.Read32(VirtIOPCICommonConfigRegisters.QueueUsedRingAddressHigh);
				return ((ulong)high << 32) | low;
			}
			set
			{
				var low = (uint)(value & 0xFFFFFFFF);
				var high = (uint)(value >> 32);
				portRange.Write32(VirtIOPCICommonConfigRegisters.QueueUsedRingAddressLow, low);
				portRange.Write32(VirtIOPCICommonConfigRegisters.QueueUsedRingAddressHigh, high);
			}
		}
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
