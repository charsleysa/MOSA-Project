﻿// Copyright (c) MOSA Project. Licensed under the New BSD License.

using System.IO;

namespace Mosa.Compiler.Linker.Elf
{
	/// <summary>
	///
	/// </summary>
	public class SymbolEntry
	{
		/// <summary>
		/// This member holds an index into the object file's symbol string table, which holds
		/// the character representations of the symbol names.
		/// </summary>
		public uint Name;

		/// <summary>
		/// This member gives the value of the associated symbol. Depending on the context,
		/// this may be an absolute value, an virtualAddress, and so on; details appear below.
		/// </summary>
		public ulong Value;

		/// <summary>
		/// Many symbols have associated sizes. For example, a data object's size is the number
		/// of bytes contained in the object. This member holds 0 if the symbol has no size or
		/// an unknown size.
		/// </summary>
		public ulong Size;

		/// <summary>
		/// The symbol binding
		/// </summary>
		public SymbolBinding SymbolBinding;

		/// <summary>
		/// The symbol type
		/// </summary>
		public SymbolType SymbolType;

		/// <summary>
		/// The symbol visibility
		/// </summary>
		public SymbolVisibility SymbolVisibility;

		/// <summary>
		/// Every symbol table entry is "defined'' in relation to some section; this member holds
		/// the relevant section header table index.
		/// </summary>
		public ushort SectionHeaderTableIndex;

		/// <summary>
		/// Gets the Info value.
		/// </summary>
		public byte Info { get { return (byte)((((byte)SymbolBinding) << 4) | (((byte)SymbolType) & 0xF)); } }

		/// <summary>
		/// This member currently holds 0 and has no defined meaning.
		/// </summary>
		public byte Other { get { return (byte)(((byte)SymbolVisibility) & 0x3); } }

		/// <summary>
		/// Writes the program header
		/// </summary>
		/// <param name="elfType">Type of the elf.</param>
		/// <param name="writer">The writer.</param>
		public void Write(ElfType elfType, BinaryWriter writer)
		{
			if (elfType == ElfType.Elf32)
				Write32(writer);
			else if (elfType == ElfType.Elf64)
				Write64(writer);
		}

		/// <summary>
		/// Writes the symbol table entry
		/// </summary>
		/// <param name="writer">The writer.</param>
		protected void Write32(BinaryWriter writer)
		{
			writer.Write((uint)Name);
			writer.Write((uint)Value);
			writer.Write((uint)Size);
			writer.Write((byte)Info);
			writer.Write((byte)Other);
			writer.Write((ushort)SectionHeaderTableIndex);
		}

		/// <summary>
		/// Writes the symbol table entry
		/// </summary>
		/// <param name="writer">The writer.</param>
		protected void Write64(BinaryWriter writer)
		{
			writer.Write((uint)Name);
			writer.Write((byte)Info);
			writer.Write((byte)Other);
			writer.Write((ushort)SectionHeaderTableIndex);
			writer.Write((ulong)Value);
			writer.Write((ulong)Size);
		}
	}
}
