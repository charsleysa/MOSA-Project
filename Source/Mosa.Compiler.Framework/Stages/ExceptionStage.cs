// Copyright (c) MOSA Project. Licensed under the New BSD License.

using Mosa.Compiler.Common;
using Mosa.Compiler.MosaTypeSystem;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Mosa.Compiler.Framework.Stages
{
	/// <summary>
	/// Exception Stage
	/// </summary>
	/// <seealso cref="Mosa.Compiler.Framework.BaseMethodCompilerStage" />
	public class ExceptionStage : BaseCodeTransformationStage
	{
		private Operand nullOperand;

		private Dictionary<BasicBlock, Operand> exceptionVirtualRegisters;
		private Dictionary<BasicBlock, Operand> leaveTargetVirtualRegisters;

		private List<BasicBlock> leaveTargets;

		private MosaMethod exceptionHandler;
		private Operand exceptionHandlerMethod;

		private delegate void Dispatch(Context context);

		protected override void PopulateVisitationDictionary()
		{
			AddVisitation(IRInstruction.Throw, Throw);
			AddVisitation(IRInstruction.FinallyStart, FinallyStart);
			AddVisitation(IRInstruction.FinallyEnd, FinallyEnd);
			AddVisitation(IRInstruction.ExceptionStart, ExceptionStart);
			AddVisitation(IRInstruction.ExceptionEnd, ExceptionEnd);
			AddVisitation(IRInstruction.Flow, Empty);
			AddVisitation(IRInstruction.TryStart, Empty);
			AddVisitation(IRInstruction.TryEnd, TryEnd);
		}

		protected override void Initialize()
		{
			base.Initialize();

			nullOperand = Operand.GetNullObject(TypeSystem);
		}

		protected override void Setup()
		{
			exceptionVirtualRegisters = new Dictionary<BasicBlock, Operand>();
			leaveTargetVirtualRegisters = new Dictionary<BasicBlock, Operand>();
			leaveTargets = new List<BasicBlock>();

			if (exceptionHandler == null)
			{
				exceptionHandler = PlatformInternalRuntimeType.FindMethodByName("ExceptionHandler");
				exceptionHandlerMethod = Operand.CreateSymbolFromMethod(exceptionHandler, TypeSystem);
			}

			CollectLeaveTargets();
		}

		protected override void Finish()
		{
			exceptionVirtualRegisters = null;
			leaveTargetVirtualRegisters = null;
			leaveTargets = null;
		}

		private static void Empty(Context context)
		{
			context.Empty();
		}

		private void ExceptionEnd(Context context)
		{
			var label = TraverseBackToNativeBlock(context.Block).Label;
			var target = context.BranchTargets[0];

			var immediate = FindImmediateExceptionHandler(label);

			Debug.Assert(immediate != null);
			Debug.Assert(immediate.ExceptionHandlerType == ExceptionHandlerType.Exception);

			var handler = FindNextEnclosingFinallyHandler(immediate);

			if (handler == null)
			{
				context.SetInstruction(IRInstruction.Jmp, target);
				return;
			}

			var handlerBlock = BasicBlocks.GetByLabel(handler.HandlerStart);

			context.SetInstruction(IRInstruction.MoveObject, LeaveTargetRegister, CreateConstant32(target.Label));
			context.AppendInstruction(IRInstruction.MoveObject, ExceptionRegister, nullOperand);
			context.AppendInstruction(IRInstruction.Jmp, handlerBlock);
		}

		private void TryEnd(Context context)
		{
			var label = TraverseBackToNativeBlock(context.Block).Label;
			var immediate = FindImmediateExceptionHandler(label);
			var target = context.BranchTargets[0];

			Debug.Assert(immediate != null);

			if (immediate.ExceptionHandlerType == ExceptionHandlerType.Finally)
			{
				context.SetInstruction(IRInstruction.MoveObject, LeaveTargetRegister, CreateConstant32(target.Label));
				context.AppendInstruction(IRInstruction.MoveObject, ExceptionRegister, nullOperand);
				context.AppendInstruction(IRInstruction.Jmp, BasicBlocks.GetByLabel(immediate.HandlerStart));
				return;
			}

			// fixme --- jump to target unless, there is a finally before it.

			var next = FindNextEnclosingFinallyHandler(immediate);

			if (next != null && next.FilterStart < immediate.HandlerEnd)
			{
				context.SetInstruction(IRInstruction.MoveObject, LeaveTargetRegister, CreateConstant32(target.Label));
				context.AppendInstruction(IRInstruction.MoveObject, ExceptionRegister, nullOperand);
				context.AppendInstruction(IRInstruction.Jmp, BasicBlocks.GetByLabel(next.HandlerStart));
				return;
			}

			context.SetInstruction(IRInstruction.Jmp, target);
		}

		private void ExceptionStart(Context context)
		{
			var exceptionVirtualRegister = context.Result;

			context.SetInstruction(IRInstruction.KillAll);
			context.AppendInstruction(IRInstruction.Gen, ExceptionRegister);
			context.AppendInstruction(IRInstruction.MoveObject, exceptionVirtualRegister, ExceptionRegister);
		}

		private void FinallyStart(Context context)
		{
			// Remove from header blocks
			BasicBlocks.RemoveHeaderBlock(context.Block);

			var exceptionVirtualRegister = context.Result;
			var leaveTargetVirtualRegister = context.Result2;

			context.SetInstruction(IRInstruction.KillAll);
			context.AppendInstruction(IRInstruction.Gen, ExceptionRegister);
			context.AppendInstruction(IRInstruction.Gen, LeaveTargetRegister);

			context.AppendInstruction(IRInstruction.MoveObject, exceptionVirtualRegister, ExceptionRegister);
			context.AppendInstruction(IRInstruction.MoveObject, leaveTargetVirtualRegister, LeaveTargetRegister);

			exceptionVirtualRegisters.Add(context.Block, exceptionVirtualRegister);
			leaveTargetVirtualRegisters.Add(context.Block, leaveTargetVirtualRegister);
		}

		private void FinallyEnd(Context context)
		{
			var naturalBlock = TraverseBackToNativeBlock(context.Block);
			var handler = FindImmediateExceptionHandler(naturalBlock.Label);

			var handlerBlock = BasicBlocks.GetByLabel(handler.HandlerStart);

			var exceptionVirtualRegister = exceptionVirtualRegisters[handlerBlock];
			var leaveTargetRegister = leaveTargetVirtualRegisters[handlerBlock];

			List<BasicBlock> targets = null;

			foreach (var target in leaveTargets)
			{
				if (target.Label <= naturalBlock.Label)
					continue;

				if (targets == null)
					targets = new List<BasicBlock>();

				targets.Add(target);
			}

			if (targets == null)
			{
				context.SetInstruction(IRInstruction.MoveObject, ExceptionRegister, exceptionVirtualRegister);
				context.AppendInstruction(IRInstruction.CallStatic, null, Operand.CreateSymbolFromMethod(exceptionHandler, TypeSystem));

				MethodScanner.MethodInvoked(exceptionHandler, Method);
				return;
			}

			var newBlocks = CreateNewBlockContexts(targets.Count, context.Label);

			var nextBlock = (targets.Count == 1) ? targets[0] : newBlocks[1].Block;

			context.SetInstruction(BranchInstruction, ConditionCode.NotEqual, null, exceptionVirtualRegister, nullOperand, newBlocks[0].Block);
			context.AppendInstruction(IRInstruction.Jmp, nextBlock);

			newBlocks[0].AppendInstruction(IRInstruction.MoveObject, ExceptionRegister, exceptionVirtualRegister);
			newBlocks[0].AppendInstruction(IRInstruction.CallStatic, null, Operand.CreateSymbolFromMethod(exceptionHandler, TypeSystem));

			MethodScanner.MethodInvoked(exceptionHandler, Method);

			int lastTarget = targets.Count - 1;

			for (int i = 0; i < lastTarget; i++)
			{
				newBlocks[i + 1].AppendInstruction(BranchInstruction, ConditionCode.Equal, null, leaveTargetRegister, CreateConstant32(targets[i].Label), targets[i]); // TODO: Constant should be 64bit
				newBlocks[i + 1].AppendInstruction(IRInstruction.Jmp, (i == lastTarget - 1) ? targets[lastTarget] : newBlocks[i + 2].Block);
			}
		}

		private void Throw(Context context)
		{
			var method = PlatformInternalRuntimeType.FindMethodByName("ExceptionHandler");

			context.SetInstruction(IRInstruction.MoveObject, ExceptionRegister, context.Operand1);
			context.AppendInstruction(IRInstruction.CallStatic, null, Operand.CreateSymbolFromMethod(method, TypeSystem));

			MethodScanner.MethodInvoked(method, Method);
		}

		private void CollectLeaveTargets()
		{
			foreach (var block in BasicBlocks)
			{
				var node = block.BeforeLast;

				while (node.IsEmptyOrNop || node.Instruction == IRInstruction.Flow)
				{
					node = node.Previous;
				}

				if (node.Instruction == IRInstruction.ExceptionEnd || node.Instruction == IRInstruction.TryEnd)
				{
					//var target = TraverseBackToNativeBlock(block);
					//var exceptionHandler = FindImmediateExceptionHandler(target.Label);
					//var handlerBlock = BasicBlocks.GetByLabel(exceptionHandler.HandlerStart);

					//leaveTargetsBySource.Add(handlerBlock, node.BranchTargets[0]);

					leaveTargets.AddIfNew(node.BranchTargets[0]);
				}
			}
		}
	}
}
