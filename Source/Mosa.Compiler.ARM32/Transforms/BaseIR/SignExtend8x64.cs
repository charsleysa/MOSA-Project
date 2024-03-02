// Copyright (c) MOSA Project. Licensed under the New BSD License.

using Mosa.Compiler.Framework;

namespace Mosa.Compiler.ARM32.Transforms.BaseIR;

/// <summary>
/// SignExtend8x64
/// </summary>
public sealed class SignExtend8x64 : BaseIRTransform
{
	public SignExtend8x64() : base(IR.SignExtend8x64, TransformType.Manual | TransformType.Transform)
	{
	}

	public override void Transform(Context context, Transform transform)
	{
		transform.SplitOperand(context.Result, out var resultLow, out var resultHigh);

		var op1 = MoveConstantToRegister(transform, context, context.Operand1);

		context.SetInstruction(ARM32.Lsl, resultLow, op1, Operand.Constant32_24);
		context.AppendInstruction(ARM32.Asr, resultLow, resultLow, Operand.Constant32_24);
		context.AppendInstruction(ARM32.Asr, resultHigh, resultLow, Operand.Constant32_31);
	}
}