// Copyright (c) MOSA Project. Licensed under the New BSD License.

using System;
using Mosa.DeviceSystem;
using Mosa.DeviceSystem.PCI;

namespace Mosa.DeviceDriver.PCI.VirtIO
{
	/// <summary>
	/// VirtIO Block Device Driver
	/// </summary>
	//[PCIDeviceDriver(VendorID = 0x1AF4, DeviceID = 0x1001, Platforms = PlatformArchitecture.X86AndX64)]
	public class VirtIOBlock : BaseDeviceDriver, IDiskDevice
	{
		internal struct VirtIOBlockFeatureFlags
		{
			internal const uint SizeMax = 1 << 1;
			internal const uint SegmentMax = 1 << 2;
			internal const uint Geometry = 1 << 4;
			internal const uint ReadOnly = 1 << 5;
			internal const uint BlockSize = 1 << 6;
			internal const uint Flush = 1 << 9;
			internal const uint Topology = 1 << 10;
			internal const uint ConfigWCE = 1 << 11;
		}

		private struct VirtIOUsedItem
		{
			public uint index;
			public uint length;
		}

		private struct QueueBuffer
		{
			public ulong address;
			public uint length;
			public ushort flags;
			public ushort next;
		}

		/// <summary>
		/// uint
		/// </summary>
		protected IOPortRead deviceFeatures;

		/// <summary>
		/// uint
		/// </summary>
		protected IOPortWrite guestFeatures;

		/// <summary>
		/// uint
		/// </summary>
		protected IOPortWrite queueAddress;

		/// <summary>
		/// ushort
		/// </summary>
		protected IOPortRead queueSize;

		/// <summary>
		/// ushort
		/// </summary>
		protected IOPortWrite queueSelect;

		/// <summary>
		/// ushort
		/// </summary>
		protected IOPortWrite queueNotify;

		/// <summary>
		/// byte
		/// </summary>
		protected IOPortReadWrite deviceStatus;

		/// <summary>
		/// byte
		/// </summary>
		protected IOPortReadWrite isrStatus;

		/// <summary>
		/// uint
		/// </summary>
		protected IOPortRead blockCountLow;

		/// <summary>
		/// uint
		/// </summary>
		protected IOPortRead blockCountHigh;

		protected ulong blockCount;

		public bool CanWrite => throw new System.NotImplementedException();

		public uint TotalBlocks => throw new System.NotImplementedException();

		public uint BlockSize => throw new System.NotImplementedException();

		public override void Initialize()
		{
			Device.Name = "VirtIOBlock_0x" + Device.Resources.GetIOPortRegion(0).BaseIOPort.ToString("X");

			deviceFeatures = Device.Resources.GetIOPortRead(0, VirtIORegisters.DeviceFeatures);
			guestFeatures = Device.Resources.GetIOPortWrite(0, VirtIORegisters.GuestFeatures);
			queueAddress = Device.Resources.GetIOPortWrite(0, VirtIORegisters.QueueAddress);
			queueSize = Device.Resources.GetIOPortRead(0, VirtIORegisters.QueueSize);
			queueSelect = Device.Resources.GetIOPortWrite(0, VirtIORegisters.QueueSelect);
			queueNotify = Device.Resources.GetIOPortWrite(0, VirtIORegisters.QueueNotify);
			deviceStatus = Device.Resources.GetIOPortReadWrite(0, VirtIORegisters.DeviceStatus);
			isrStatus = Device.Resources.GetIOPortReadWrite(0, VirtIORegisters.ISRStatus);
			blockCountLow = Device.Resources.GetIOPortRead(0, 0x14);
			blockCountHigh = Device.Resources.GetIOPortRead(0, 0x18);
		}

		public override void Probe()
		{
			// Find common config
			var pciDevice = Device.Parent.DeviceDriver as PCIDevice;

			PCICapability common;
			var commonFound = false;
			foreach (var cap in pciDevice.Capabilities)
			{
				if (cap.capabilityID != PCICapabilityID.VNDR) continue;

				var bar = pciDevice.ReadConfig8(VirtIOPCICapabilityRegisters.BAR);

				// Ignore structures with reserved BAR values
				if (bar > 0x05) continue;

				var type = pciDevice.ReadConfig8(VirtIOPCICapabilityRegisters.ConfigType);

				// Make sure type matches
				if (type != VirtIOPCICapabilityType.CommonConfig) continue;
			}

			// Get block count (ulong)
			blockCount = blockCountHigh.Read32() << 32;
			blockCount = blockCountLow.Read32() | blockCount;

			// Reset device
			deviceStatus.Write8(0);

			// Tell device that we are loaded
			deviceStatus.Write8(VirtIOStatusFlags.DeviceAcknowledged);
			deviceStatus.Write8(VirtIOStatusFlags.DeviceAcknowledged | VirtIOStatusFlags.DriverLoaded);

			// Negotiate features
			var features = deviceFeatures.Read32();
			features = features & ~(VirtIOBlockFeatureFlags.ReadOnly | VirtIOBlockFeatureFlags.BlockSize | VirtIOBlockFeatureFlags.Topology);
			guestFeatures.Write32(features);

			// Check if device accepted features
			deviceStatus.Write8(VirtIOStatusFlags.DeviceAcknowledged | VirtIOStatusFlags.DriverLoaded | VirtIOStatusFlags.FeaturesAccepted);
			if ((deviceStatus.Read8() & VirtIOStatusFlags.FeaturesAccepted) == 0)
				Device.Status = DeviceStatus.NotFound; // TODO: should this be notfound or error?
			else
				Device.Status = DeviceStatus.Available;
		}

		public unsafe override void Start()
		{
			var alignment = 32u;

			// Get queue size
			queueSelect.Write16(0);
			var queueSize = this.queueSize.Read16();
			var sizeOfBuffers = (uint)sizeof(QueueBuffer) * queueSize;
			var sizeOfQueueAvailable = sizeof(ushort) * (3u + queueSize);
			var sizeOfQueueUsed = sizeof(ushort) * 3u + (uint)sizeof(VirtIOUsedItem) * queueSize;
			var alignedSizeOfBuffersAndQueueAvailable = (sizeOfBuffers + sizeOfQueueAvailable + alignment - 1) & ~(alignment - 1);
			var buffer = HAL.AllocateMemory(alignedSizeOfBuffersAndQueueAvailable + sizeOfQueueUsed, alignment);

			var ptrBuffers = buffer.Address;
			var ptrAvailable = ptrBuffers + (int)sizeOfBuffers;
			var ptrUsed = ptrBuffers + (int)alignedSizeOfBuffersAndQueueAvailable;
		}

		public byte[] ReadBlock(uint block, uint count)
		{
			throw new System.NotImplementedException();
		}

		public bool ReadBlock(uint block, uint count, byte[] data)
		{
			throw new System.NotImplementedException();
		}

		public bool WriteBlock(uint block, uint count, byte[] data)
		{
			throw new System.NotImplementedException();
		}
	}
}
