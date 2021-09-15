﻿// Copyright (c) MOSA Project. Licensed under the New BSD License.

namespace Mosa.Compiler.Framework.Transform.Manual.ConstantFolding
{
	public sealed class Compare32x64 : BaseTransformation
	{
		public Compare32x64() : base(IRInstruction.Compare32x64)
		{
		}

		public override bool Match(Context context, TransformContext transformContext)
		{
			if (!IsResolvedConstant(context.Operand1))
				return false;

			if (!IsResolvedConstant(context.Operand2))
				return false;

			switch (context.ConditionCode)
			{
				case ConditionCode.Equal: return true;
				case ConditionCode.NotEqual: return true;
				case ConditionCode.GreaterOrEqual: return true;
				case ConditionCode.Greater: return true;
				case ConditionCode.LessOrEqual: return true;
				case ConditionCode.Less: return true;
				case ConditionCode.UnsignedGreater: return true;
				case ConditionCode.UnsignedGreaterOrEqual: return true;
				case ConditionCode.UnsignedLess: return true;
				case ConditionCode.UnsignedLessOrEqual: return true;
				default: return false;
			}
		}

		public override void Transform(Context context, TransformContext transformContext)
		{
			var compare = Compare32(context);

			var e1 = transformContext.CreateConstant(BoolTo64(compare));

			context.SetInstruction(IRInstruction.Move64, context.Result, e1);
		}
	}
}
