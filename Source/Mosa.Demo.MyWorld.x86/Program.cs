// Copyright (c) MOSA Project. Licensed under the New BSD License.

using Mosa.Kernel.x86;

namespace Mosa.Demo.MyWorld.x86
{
	public static class Program
	{
		public unsafe static void Setup()
		{
			int[] array = new int[10] { 4, 2, 7, 3, 6, 9, 1, 0, 2, 8 };
			fixed (int* ptr = array)
			{
				System.Span<int> span = new System.Span<int>(ptr, array.Length);
				Screen.WriteLine("Created span");

				span = span.Slice(2, 4);
				Screen.WriteLine("Sliced span");
			}
		}

		public static void Loop()
		{ }

		public static void OnInterrupt()
		{ }
	}
}
