// Copyright (c) MOSA Project. Licensed under the New BSD License.

using Mosa.Kernel.x86;

namespace Mosa.CoolWorld.x86.HAL
{
	/// <summary>
	/// X86IOPortReadWrite
	/// </summary>
	/// <seealso cref="Mosa.DeviceSystem.IOPortReadWrite" />
	public sealed class X86IOPortReadWrite : DeviceSystem.IOPortReadWrite
	{
		public X86IOPortReadWrite(ushort address)
		{
			Address = address;
		}

		/// <summary>
		/// Read8s this instance.
		/// </summary>
		/// <returns></returns>
		public override byte Read8()
		{
			return IOPort.In8(Address);
		}

		/// <summary>
		/// Read16s this instance.
		/// </summary>
		/// <returns></returns>
		public override ushort Read16()
		{
			return IOPort.In16(Address);
		}

		/// <summary>
		/// Read32s this instance.
		/// </summary>
		/// <returns></returns>
		public override uint Read32()
		{
			return IOPort.In32(Address);
		}

		/// <summary>
		/// Write8s the specified data.
		/// </summary>
		/// <param name="data">The data.</param>
		public override void Write8(byte data)
		{
			IOPort.Out8(Address, data);
		}

		/// <summary>
		/// Write16s the specified data.
		/// </summary>
		/// <param name="data">The data.</param>
		public override void Write16(ushort data)
		{
			IOPort.Out16(Address, data);
		}

		/// <summary>
		/// Write32s the specified data.
		/// </summary>
		/// <param name="data">The data.</param>
		public override void Write32(uint data)
		{
			IOPort.Out32(Address, data);
		}
	}

	/// <summary>
	/// X86IOPortRead
	/// </summary>
	/// <seealso cref="Mosa.DeviceSystem.IOPortRead" />
	public sealed class X86IOPortRead : DeviceSystem.IOPortRead
	{
		public X86IOPortRead(ushort address)
		{
			Address = address;
		}

		/// <summary>
		/// Read8s this instance.
		/// </summary>
		/// <returns></returns>
		public override byte Read8()
		{
			return IOPort.In8(Address);
		}

		/// <summary>
		/// Read16s this instance.
		/// </summary>
		/// <returns></returns>
		public override ushort Read16()
		{
			return IOPort.In16(Address);
		}

		/// <summary>
		/// Read32s this instance.
		/// </summary>
		/// <returns></returns>
		public override uint Read32()
		{
			return IOPort.In32(Address);
		}
	}

	/// <summary>
	/// X86IOPortWrite
	/// </summary>
	/// <seealso cref="Mosa.DeviceSystem.IOPortWrite" />
	public sealed class X86IOPortWrite : DeviceSystem.IOPortWrite
	{
		public X86IOPortWrite(ushort address)
		{
			Address = address;
		}

		/// <summary>
		/// Write8s the specified data.
		/// </summary>
		/// <param name="data">The data.</param>
		public override void Write8(byte data)
		{
			IOPort.Out8(Address, data);
		}

		/// <summary>
		/// Write16s the specified data.
		/// </summary>
		/// <param name="data">The data.</param>
		public override void Write16(ushort data)
		{
			IOPort.Out16(Address, data);
		}

		/// <summary>
		/// Write32s the specified data.
		/// </summary>
		/// <param name="data">The data.</param>
		public override void Write32(uint data)
		{
			IOPort.Out32(Address, data);
		}
	}

	/// <summary>
	/// X86IOPortReadWriteRange
	/// </summary>
	/// <seealso cref="Mosa.DeviceSystem.IOPortReadWrite" />
	public sealed class X86IOPortReadWriteRange : DeviceSystem.IOPortReadWriteRange
	{
		public X86IOPortReadWriteRange(ushort address, ushort length)
		{
			Address = address;
			Length = length;
		}

		/// <summary>
		/// Read8s this instance.
		/// </summary>
		/// <param name="offset">The offset within the range.</param>
		/// <returns></returns>
		public override byte Read8(ushort offset)
		{
			return IOPort.In8((ushort)(Address + offset));
		}

		/// <summary>
		/// Read16s this instance.
		/// </summary>
		/// <param name="offset">The offset within the range.</param>
		/// <returns></returns>
		public override ushort Read16(ushort offset)
		{
			return IOPort.In16((ushort)(Address + offset));
		}

		/// <summary>
		/// Read32s this instance.
		/// </summary>
		/// <param name="offset">The offset within the range.</param>
		/// <returns></returns>
		public override uint Read32(ushort offset)
		{
			return IOPort.In32((ushort)(Address + offset));
		}

		/// <summary>
		/// Write8s the specified data.
		/// </summary>
		/// <param name="offset">The offset within the range.</param>
		/// <param name="data">The data.</param>
		public override void Write8(ushort offset, byte data)
		{
			IOPort.Out8((ushort)(Address + offset), data);
		}

		/// <summary>
		/// Write16s the specified data.
		/// </summary>
		/// <param name="offset">The offset within the range.</param>
		/// <param name="data">The data.</param>
		public override void Write16(ushort offset, ushort data)
		{
			IOPort.Out16((ushort)(Address + offset), data);
		}

		/// <summary>
		/// Write32s the specified data.
		/// </summary>
		/// <param name="offset">The offset within the range.</param>
		/// <param name="data">The data.</param>
		public override void Write32(ushort offset, uint data)
		{
			IOPort.Out32((ushort)(Address + offset), data);
		}
	}
}
