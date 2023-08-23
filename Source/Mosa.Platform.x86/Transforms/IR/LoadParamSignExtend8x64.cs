// Copyright (c) MOSA Project. Licensed under the New BSD License.

using Mosa.Compiler.Framework;

namespace Mosa.Platform.x86.Transforms.IR;

/// <summary>
/// LoadParamSignExtend8x64
/// </summary>
[Transform("x86.IR")]
public sealed class LoadParamSignExtend8x64 : BaseIRTransform
{
	public LoadParamSignExtend8x64() : base(IRInstruction.LoadParamSignExtend8x64, TransformType.Manual | TransformType.Transform)
	{
	}

	public override void Transform(Context context, TransformContext transform)
	{
		transform.SplitOperand(context.Result, out var resultLow, out var resultHigh);
		transform.SplitOperand(context.Operand1, out var lowOffset, out _);

		context.SetInstruction(X86.MovsxLoad8, resultLow, transform.StackFrame, lowOffset);
		context.AppendInstruction(X86.Cdq32, resultHigh, resultLow);
	}
}