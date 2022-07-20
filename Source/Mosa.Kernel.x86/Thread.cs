// Copyright (c) MOSA Project. Licensed under the New BSD License.

using Mosa.Runtime;
using System.Threading;

namespace Mosa.Kernel.x86
{
	internal class Thread
	{
		public ThreadState Status = ThreadState.Unstarted;
		public Pointer StackTop;
		public Pointer StackBottom;
		public Pointer StackStatePointer;
		public uint Ticks;
	}
}
