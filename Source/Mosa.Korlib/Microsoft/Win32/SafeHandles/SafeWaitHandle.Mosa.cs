// Copyright (c) MOSA Project. Licensed under the New BSD License.

using System.Threading;

namespace Microsoft.Win32.SafeHandles
{
	public sealed partial class SafeWaitHandle : SafeHandleZeroOrMinusOneIsInvalid
	{
		protected override bool ReleaseHandle()
		{
			return true;
		}
	}
}
