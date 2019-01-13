// Copyright (c) MOSA Project. Licensed under the New BSD License.

// This code was generated by an automated template.

using Mosa.Compiler.Framework;

namespace Mosa.Platform.x64.Instructions
{
	/// <summary>
	/// Bt64
	/// </summary>
	/// <seealso cref="Mosa.Platform.x64.X64Instruction" />
	public sealed class Bt64 : X64Instruction
	{
		public override int ID { get { return 383; } }

		internal Bt64()
			: base(1, 2)
		{
		}

		public override bool IsCarryFlagModified { get { return true; } }

		public override bool IsSignFlagUnchanged { get { return true; } }

		public override bool IsSignFlagUndefined { get { return true; } }

		public override bool IsOverflowFlagUnchanged { get { return true; } }

		public override bool IsOverflowFlagUndefined { get { return true; } }

		public override bool IsParityFlagUnchanged { get { return true; } }

		public override bool IsParityFlagUndefined { get { return true; } }

		public override void Emit(InstructionNode node, BaseCodeEmitter emitter)
		{
			System.Diagnostics.Debug.Assert(node.ResultCount == 1);
			System.Diagnostics.Debug.Assert(node.OperandCount == 2);

			if (node.Operand2.IsCPURegister)
			{
				emitter.OpcodeEncoder.SuppressByte(0x40);
				emitter.OpcodeEncoder.AppendNibble(0b0100);
				emitter.OpcodeEncoder.AppendBit(0b1);
				emitter.OpcodeEncoder.AppendBit((node.Result.Register.RegisterCode >> 3) & 0x1);
				emitter.OpcodeEncoder.AppendBit(0b0);
				emitter.OpcodeEncoder.AppendBit((node.Operand2.Register.RegisterCode >> 3) & 0x1);
				emitter.OpcodeEncoder.AppendByte(0x0F);
				emitter.OpcodeEncoder.AppendByte(0xA3);
				emitter.OpcodeEncoder.Append2Bits(0b11);
				emitter.OpcodeEncoder.Append3Bits(node.Result.Register.RegisterCode);
				emitter.OpcodeEncoder.Append3Bits(node.Operand2.Register.RegisterCode);
				return;
			}

			if (node.Operand2.IsConstant)
			{
				emitter.OpcodeEncoder.SuppressByte(0x40);
				emitter.OpcodeEncoder.AppendNibble(0b0100);
				emitter.OpcodeEncoder.AppendBit(0b1);
				emitter.OpcodeEncoder.AppendBit((node.Result.Register.RegisterCode >> 3) & 0x1);
				emitter.OpcodeEncoder.AppendBit(0b0);
				emitter.OpcodeEncoder.AppendBit(0b0);
				emitter.OpcodeEncoder.AppendByte(0x0F);
				emitter.OpcodeEncoder.AppendByte(0xBA);
				emitter.OpcodeEncoder.Append2Bits(0b11);
				emitter.OpcodeEncoder.Append3Bits(0b100);
				emitter.OpcodeEncoder.Append3Bits(node.Result.Register.RegisterCode);
				emitter.OpcodeEncoder.Append8BitImmediate(node.Operand2);
				return;
			}

			throw new Compiler.Common.Exceptions.CompilerException("Invalid Opcode");
		}
	}
}
