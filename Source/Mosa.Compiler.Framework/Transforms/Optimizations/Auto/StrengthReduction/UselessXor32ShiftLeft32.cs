// Copyright (c) MOSA Project. Licensed under the New BSD License.

// This code was generated by an automated template.

using Mosa.Compiler.Framework;

namespace Mosa.Compiler.Framework.Transforms.Optimizations.Auto.StrengthReduction;

/// <summary>
/// UselessXor32ShiftLeft32
/// </summary>
[Transform("IR.Optimizations.Auto.StrengthReduction")]
public sealed class UselessXor32ShiftLeft32 : BaseTransform
{
	public UselessXor32ShiftLeft32() : base(IRInstruction.ShiftLeft32, TransformType.Auto | TransformType.Optimization)
	{
	}

	public override bool Match(Context context, TransformContext transform)
	{
		if (!context.Operand1.IsVirtualRegister)
			return false;

		if (!context.Operand1.IsDefinedOnce)
			return false;

		if (context.Operand1.Definitions[0].Instruction != IRInstruction.Xor32)
			return false;

		if (!IsConstant(context.Operand1.Definitions[0].Operand2))
			return false;

		if (!IsConstant(context.Operand2))
			return false;

		if (IsZero(context.Operand2))
			return false;

		if (!IsGreaterOrEqual(CountTrailingZeros(To32(context.Operand1.Definitions[0].Operand2)), To32(context.Operand2)))
			return false;

		return true;
	}

	public override void Transform(Context context, TransformContext transform)
	{
		var result = context.Result;

		var t1 = context.Operand1.Definitions[0].Operand1;
		var t2 = context.Operand2;

		context.SetInstruction(IRInstruction.ShiftLeft32, result, t1, t2);
	}
}

/// <summary>
/// UselessXor32ShiftLeft32_v1
/// </summary>
[Transform("IR.Optimizations.Auto.StrengthReduction")]
public sealed class UselessXor32ShiftLeft32_v1 : BaseTransform
{
	public UselessXor32ShiftLeft32_v1() : base(IRInstruction.ShiftLeft32, TransformType.Auto | TransformType.Optimization)
	{
	}

	public override bool Match(Context context, TransformContext transform)
	{
		if (!context.Operand1.IsVirtualRegister)
			return false;

		if (!context.Operand1.IsDefinedOnce)
			return false;

		if (context.Operand1.Definitions[0].Instruction != IRInstruction.Xor32)
			return false;

		if (!IsConstant(context.Operand1.Definitions[0].Operand1))
			return false;

		if (!IsConstant(context.Operand2))
			return false;

		if (IsZero(context.Operand2))
			return false;

		if (!IsGreaterOrEqual(CountTrailingZeros(To32(context.Operand1.Definitions[0].Operand1)), To32(context.Operand2)))
			return false;

		return true;
	}

	public override void Transform(Context context, TransformContext transform)
	{
		var result = context.Result;

		var t1 = context.Operand1.Definitions[0].Operand2;
		var t2 = context.Operand2;

		context.SetInstruction(IRInstruction.ShiftLeft32, result, t1, t2);
	}
}