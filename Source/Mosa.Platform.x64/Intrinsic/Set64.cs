// Copyright (c) MOSA Project. Licensed under the New BSD License.

using Mosa.Compiler.Framework;

namespace Mosa.Platform.x64.Intrinsic
{
	/// <summary>
	/// IntrinsicMethods
	/// </summary>
	internal static partial class IntrinsicMethods
	{
		[IntrinsicMethod("Mosa.Platform.x64.Intrinsic::Set64")]
		private static void Set64(Context context, MethodCompiler methodCompiler)
		{
			context.SetInstruction(X64.MovStore64, null, context.Operand1, methodCompiler.ConstantZero32, context.Operand2);
		}
	}
}
