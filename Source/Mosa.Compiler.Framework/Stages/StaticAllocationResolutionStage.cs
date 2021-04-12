// Copyright (c) MOSA Project. Licensed under the New BSD License.

using Mosa.Compiler.Common.Exceptions;
using Mosa.Compiler.Framework.Linker;

namespace Mosa.Compiler.Framework.Stages
{
	public class StaticAllocationResolutionStage : BaseMethodCompilerStage
	{
		private int allocationCount = 0;
		public const string StaticSymbolPrefix = "$cctor$";

		protected override void Run()
		{
			if (!MethodCompiler.IsCILStream)
				return;

			if (!Method.IsTypeConstructor)
				return;

			foreach (var block in BasicBlocks)
			{
				for (var node = block.AfterFirst; !node.IsBlockEndInstruction; node = node.Next)
				{
					if (node.IsEmptyOrNop)
						continue;

					if (node.Instruction == IRInstruction.NewObject || node.Instruction == IRInstruction.NewArray)
					{
						PerformStaticAllocation(new Context(node));
					}
				}
			}
		}

		private void PerformStaticAllocation(Context context)
		{
			var allocatedType = context.MosaType; // node.Result.Type;

			bool newObject = context.Instruction == IRInstruction.NewObject;
			uint elements = 0;

			//Debug.WriteLine($"Method: {Method} : {node}");
			//Debug.WriteLine($"  --> {allocatedType}");

			MethodScanner.TypeAllocated(allocatedType, Method);

			uint allocationSize;

			if (newObject)
			{
				allocationSize = TypeLayout.GetTypeSize(allocatedType) + (TypeLayout.NativePointerSize * 2);
			}
			else
			{
				elements = (uint)GetConstant(context.Operand3);
				allocationSize = (TypeLayout.GetTypeSize(allocatedType.ElementType) * elements) + (TypeLayout.NativePointerSize * 3);
			}

			// Ensure unique symbol name
			var symbolName = Linker.DefineSymbol($"{StaticSymbolPrefix}{Method.DeclaringType.FullName}${allocationCount++}", SectionKind.BSS, Architecture.NativeAlignment, allocationSize);

			string typeDefinitionSymbol = Metadata.TypeDefinition + allocatedType.FullName;

			Linker.Link(LinkType.AbsoluteAddress, Is32BitPlatform ? PatchType.I32 : PatchType.I64, symbolName, TypeLayout.NativePointerSize, typeDefinitionSymbol, 0);

			var staticAddress = Operand.CreateSymbol(TypeSystem.BuiltIn.Pointer, symbolName.Name);

			var move = Is32BitPlatform ? (BaseInstruction)IRInstruction.Move32 : IRInstruction.Move64;
			var add = Is32BitPlatform ? (BaseInstruction)IRInstruction.Add32 : IRInstruction.Add64;
			var store = Is32BitPlatform ? (BaseInstruction)IRInstruction.Store32 : IRInstruction.Store64;

			context.SetInstruction(move, context.Result, staticAddress);
			context.AppendInstruction(add, context.Result, context.Result, CreateConstant32(NativePointerSize * 2));

			if (!newObject)
			{
				context.AppendInstruction(store, null, context.Result, ConstantZero, CreateConstant32(elements));
			}
		}

		private static long GetConstant(Operand operand)
		{
			while (true)
			{
				if (operand.IsConstant)
					return operand.ConstantSigned64;

				if (operand.Definitions.Count != 1)
					break;

				var node = operand.Definitions[0];

				if ((node.Instruction == IRInstruction.Move32 || node.Instruction == IRInstruction.Move64) && node.Operand1.IsConstant)
				{
					operand = node.Operand1;
					continue;
				}

				break;
			}

			throw new CompilerException("unable to find constant value");
		}
	}
}
