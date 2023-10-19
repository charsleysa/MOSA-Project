﻿// Copyright (c) MOSA Project. Licensed under the New BSD License.

namespace Mosa.Compiler.Framework.Transforms.Optimizations.Manual.ConstantMove;

public sealed class BranchObject : BaseTransform
{
	public BranchObject() : base(IRInstruction.BranchObject, TransformType.Manual | TransformType.Optimization)
	{
	}

	public override bool Match(Context context, TransformContext transform)
	{
		if (IsConstant(context.Operand2))
			return false;

		if (!IsConstant(context.Operand1))
			return false;

		if (context.Block.NextBlocks.Count == 1)
			return false;

		return true;
	}

	public override void Transform(Context context, TransformContext transform)
	{
		context.SetInstruction(IRInstruction.BranchObject, context.ConditionCode.GetReverse(), context.Result, context.Operand2, context.Operand1, context.BranchTargets[0]);
	}
}