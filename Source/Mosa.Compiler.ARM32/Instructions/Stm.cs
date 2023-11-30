// Copyright (c) MOSA Project. Licensed under the New BSD License.

// This code was generated by an automated template.

using Mosa.Compiler.Framework;

namespace Mosa.Compiler.ARM32.Instructions;

/// <summary>
/// Stm - Block transfer multiple registers to memory
/// </summary>
public sealed class Stm : ARM32Instruction
{
	internal Stm()
		: base(0, 3)
	{
	}

	public override void Emit(Node node, OpcodeEncoder opcodeEncoder)
	{
		System.Diagnostics.Debug.Assert(node.ResultCount == 0);
		System.Diagnostics.Debug.Assert(node.OperandCount == 3);

		if (node.Operand1.IsPhysicalRegister && node.Operand2.IsConstant && node.Operand3.IsConstant)
		{
			opcodeEncoder.Append4Bits(GetConditionCode(node.ConditionCode));
			opcodeEncoder.Append3Bits(0b100);
			opcodeEncoder.Append1Bit(0b1);
			opcodeEncoder.Append1Bit(node.IsUpDirection ? 1 : 0);
			opcodeEncoder.Append1BitImmediate(node.Operand2);
			opcodeEncoder.Append1Bit(node.IsWriteback ? 1 : 0);
			opcodeEncoder.Append1Bit(0b0);
			opcodeEncoder.Append4Bits(node.Operand1.Register.RegisterCode);
			opcodeEncoder.Append16BitImmediate(node.Operand3);
			return;
		}

		if (node.Operand1.IsPhysicalRegister && node.Operand2.IsConstant && node.Operand3.IsPhysicalRegister)
		{
			opcodeEncoder.Append4Bits(GetConditionCode(node.ConditionCode));
			opcodeEncoder.Append3Bits(0b100);
			opcodeEncoder.Append1Bit(0b1);
			opcodeEncoder.Append1Bit(node.IsUpDirection ? 1 : 0);
			opcodeEncoder.Append1BitImmediate(node.Operand2);
			opcodeEncoder.Append1Bit(node.IsWriteback ? 1 : 0);
			opcodeEncoder.Append1Bit(0b0);
			opcodeEncoder.Append4Bits(node.Operand1.Register.RegisterCode);
			opcodeEncoder.Append16Bits(ToPower2(node.Operand3.Register.RegisterCode));
			return;
		}

		throw new Compiler.Common.Exceptions.CompilerException("Invalid Opcode");
	}
}
