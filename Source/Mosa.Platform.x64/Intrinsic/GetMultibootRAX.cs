﻿// Copyright (c) MOSA Project. Licensed under the New BSD License.

using Mosa.Compiler.Framework;
using Mosa.Compiler.Framework.Platform;

namespace Mosa.Platform.x64.Intrinsic;

/// <summary>
/// IntrinsicMethods
/// </summary>
internal static partial class IntrinsicMethods
{
	[IntrinsicMethod("Mosa.Platform.x64.Intrinsic::GetMultibootRAX")]
	private static void GetMultibootRAX(Context context, TransformContext transformContext)
	{
		var MultibootEAX = Operand.CreateLabel(BaseMultibootStage.MultibootEAX, transformContext.Is32BitPlatform);

		context.SetInstruction(IRInstruction.Load64, context.Result, MultibootEAX, Operand.Constant32_0);
	}
}