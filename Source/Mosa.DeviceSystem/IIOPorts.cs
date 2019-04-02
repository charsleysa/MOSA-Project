// Copyright (c) MOSA Project. Licensed under the New BSD License.

namespace Mosa.DeviceSystem
{
	public abstract class IOPort
	{
		/// <summary>
		/// Gets the address.
		/// </summary>
		/// <value>
		/// The address.
		/// </value>
		public ushort Address { get; protected set; }
	}

	/// <summary>
	/// Interface to IOPort with read only permission
	/// </summary>
	public abstract class IOPortRead : IOPort
	{
		/// <summary>
		/// Read8s this instance.
		/// </summary>
		/// <returns></returns>
		public abstract byte Read8();

		/// <summary>
		/// Read16s this instance.
		/// </summary>
		/// <returns></returns>
		public abstract ushort Read16();

		/// <summary>
		/// Read32s this instance.
		/// </summary>
		/// <returns></returns>
		public abstract uint Read32();
	}

	/// <summary>
	/// Interface to IOPort with write only permission
	/// </summary>
	public abstract class IOPortWrite : IOPort
	{
		/// <summary>
		/// Write8s the specified data.
		/// </summary>
		/// <param name="data">The data.</param>
		public abstract void Write8(byte data);

		/// <summary>
		/// Write16s the specified data.
		/// </summary>
		/// <param name="data">The data.</param>
		public abstract void Write16(ushort data);

		/// <summary>
		/// Write32s the specified data.
		/// </summary>
		/// <param name="data">The data.</param>
		public abstract void Write32(uint data);
	}

	/// <summary>
	/// class to IOPort with full read/write permissions
	/// </summary>
	public abstract class IOPortReadWrite : IOPortRead
	{
		/// <summary>
		/// Write8s the specified data.
		/// </summary>
		/// <param name="data">The data.</param>
		public abstract void Write8(byte data);

		/// <summary>
		/// Write16s the specified data.
		/// </summary>
		/// <param name="data">The data.</param>
		public abstract void Write16(ushort data);

		/// <summary>
		/// Write32s the specified data.
		/// </summary>
		/// <param name="data">The data.</param>
		public abstract void Write32(uint data);
	}

	/// <summary>
	/// class to IOPort with full read/write permissions for port range
	/// </summary>
	public abstract class IOPortReadWriteRange : IOPort
	{
		/// <summary>
		/// Gets the length.
		/// </summary>
		/// <value>
		/// The length.
		/// </value>
		public ushort Length { get; protected set; }

		/// <summary>
		/// Read8s this instance.
		/// </summary>
		/// <param name="offset">The offset within the range.</param>
		/// <returns></returns>
		public abstract byte Read8(ushort offset);

		/// <summary>
		/// Read16s this instance.
		/// </summary>
		/// <param name="offset">The offset within the range.</param>
		/// <returns></returns>
		public abstract ushort Read16(ushort offset);

		/// <summary>
		/// Read32s this instance.
		/// </summary>
		/// <param name="offset">The offset within the range.</param>
		/// <returns></returns>
		public abstract uint Read32(ushort offset);

		/// <summary>
		/// Write8s the specified data.
		/// </summary>
		/// <param name="offset">The offset within the range.</param>
		/// <param name="data">The data.</param>
		public abstract void Write8(ushort offset, byte data);

		/// <summary>
		/// Write16s the specified data.
		/// </summary>
		/// <param name="offset">The offset within the range.</param>
		/// <param name="data">The data.</param>
		public abstract void Write16(ushort offset, ushort data);

		/// <summary>
		/// Write32s the specified data.
		/// </summary>
		/// <param name="offset">The offset within the range.</param>
		/// <param name="data">The data.</param>
		public abstract void Write32(ushort offset, uint data);
	}
}
