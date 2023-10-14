// Copyright (c) MOSA Project. Licensed under the New BSD License.

using Mosa.Compiler.Framework;

namespace Mosa.Compiler.x64.Transforms.IR;

/// <summary>
/// BitCopy32ToR4
/// </summary>
[Transform("x64.IR")]
public sealed class BitCopy32ToR4 : BaseIRTransform
{
	public BitCopy32ToR4() : base(IRInstruction.BitCopy32ToR4, TransformType.Manual | TransformType.Transform)
	{
	}

	public override void Transform(Context context, TransformContext transform)
	{
		//context.ReplaceInstruction(X64.Movdi32ss);	// TODO
	}
}