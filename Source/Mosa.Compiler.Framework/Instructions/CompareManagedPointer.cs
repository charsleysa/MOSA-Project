// Copyright (c) MOSA Project. Licensed under the New BSD License.

// This code was generated by an automated template.

using Mosa.Compiler.MosaTypeSystem;

namespace Mosa.Compiler.Framework.Instructions;

/// <summary>
/// CompareManagedPointer
/// </summary>
public sealed class CompareManagedPointer : BaseIRInstruction
{
	public CompareManagedPointer()
		: base(2, 1)
	{
	}

	public override bool IsCompare => true;
}