// Copyright (c) MOSA Project. Licensed under the New BSD License.

using Mosa.Compiler.Framework;

namespace Mosa.Platform.x86.Transforms.IR;

/// <summary>
/// Sub32
/// </summary>
public sealed class Sub32 : BaseIRTransform
{
	public Sub32() : base(IRInstruction.Sub32, TransformType.Manual | TransformType.Transform)
	{
	}

	public override void Transform(Context context, TransformContext transform)
	{
		context.ReplaceInstruction(X86.Sub32);
	}
}
