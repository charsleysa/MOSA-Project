// Copyright (c) MOSA Project. Licensed under the New BSD License.

namespace Mosa.Compiler.Framework.Transforms.Optimizations.Manual.Useless;

/// <summary>
/// LoadParamSignExtend8x64Double
/// </summary>
[Transform("IR.Optimizations.Manual.Useless")]
public sealed class LoadParamSignExtend8x64Double : BaseTransform
{
	public LoadParamSignExtend8x64Double() : base(IRInstruction.SignExtend8x64, TransformType.Manual | TransformType.Optimization)
	{
	}

	public override int Priority => 85;

	public override bool Match(Context context, TransformContext transform)
	{
		if (!context.Operand1.IsVirtualRegister)
			return false;

		if (!context.Operand1.IsDefinedOnce)
			return false;

		if (context.Operand1.Definitions[0].Instruction != IRInstruction.LoadParamSignExtend8x64)
			return false;

		return true;
	}

	public override void Transform(Context context, TransformContext transform)
	{
		var result = context.Result;
		var operand1 = context.Operand1;

		context.SetInstruction(IRInstruction.Move64, result, operand1);
	}
}