// Copyright (c) MOSA Project. Licensed under the New BSD License.

using Mosa.Compiler.Framework;

namespace Mosa.Platform.x64.Transforms.FixedRegisters;

/// <summary>
/// IMul32Constant
/// </summary>
[Transform("x64.FixedRegisters")]
public sealed class IMul32Constant : BaseTransform
{
	public IMul32Constant() : base(X64.IMul32, TransformType.Manual | TransformType.Transform)
	{
	}

	public override bool Match(Context context, TransformContext transform)
	{
		return context.Operand2.IsConstant;
	}

	public override void Transform(Context context, TransformContext transform)
	{
		var operand2 = context.Operand2;

		var v1 = transform.VirtualRegisters.Allocate32();

		context.InsertBefore().AppendInstruction(X64.Mov32, v1, operand2);
		context.Operand2 = v1;
	}
}