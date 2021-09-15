﻿// Copyright (c) MOSA Project. Licensed under the New BSD License.

namespace Mosa.Compiler.Framework.Transform.Manual.ConstantFolding
{
	public sealed class Compare64x32 : BaseTransformation
	{
		public Compare64x32() : base(IRInstruction.Compare64x32)
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

			var e1 = transformContext.CreateConstant(BoolTo32(compare));

			context.SetInstruction(IRInstruction.Move32, context.Result, e1);
		}
	}
}
