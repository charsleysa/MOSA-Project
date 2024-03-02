// Copyright (c) MOSA Project. Licensed under the New BSD License.

// This code was generated by an automated template.

namespace Mosa.Compiler.Framework.Instructions;

/// <summary>
/// StoreManagedPointer
/// </summary>
public sealed class StoreManagedPointer : BaseIRInstruction
{
	public StoreManagedPointer()
		: base(3, 0)
	{
	}

	public override bool IsMemoryWrite => true;
}