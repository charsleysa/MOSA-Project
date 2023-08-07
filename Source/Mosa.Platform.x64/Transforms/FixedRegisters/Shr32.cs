// Copyright (c) MOSA Project. Licensed under the New BSD License.

using Mosa.Compiler.Framework;

namespace Mosa.Platform.x64.Transforms.FixedRegisters;

/// <summary>
/// Shr32
/// </summary>
[Transform("x64.FixedRegisters")]
public sealed class Shr32 : BaseTransform
{
	public Shr32() : base(X64.Shr32, TransformType.Manual | TransformType.Transform)
	{
	}


	public override bool Match(Context context, TransformContext transform)
	{
		if (context.Operand2.IsConstant)
			return false;

		if (context.Operand2.Register == CPURegister.RCX)
			return false;

		return true;
	}


	public override void Transform(Context context, TransformContext transform)
	{
		var operand1 = context.Operand1;
		var operand2 = context.Operand2;
		var result = context.Result;

		var rcx = Operand.CreateCPURegister32(CPURegister.RCX);

		context.SetInstruction(X64.Mov64, rcx, operand2);
		context.AppendInstruction(X64.Shr32, result, operand1, rcx);
	}
}
