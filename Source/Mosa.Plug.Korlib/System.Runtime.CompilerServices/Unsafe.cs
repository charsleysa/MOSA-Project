﻿// Copyright (c) MOSA Project. Licensed under the New BSD License.

using Mosa.Runtime;
using Mosa.Runtime.Plug;

namespace Mosa.Plug.Korlib.System.Runtime.CompilerServices
{
	internal static class Unsafe
	{
		[Plug("System.Runtime.CompilerServices.Unsafe::As")]
		public static object As(object value)
		{
			var p = Intrinsic.GetObjectAddress(value);
			var o = Intrinsic.GetObjectFromAddress(p);

			return o;
		}

		//[Plug("System.Runtime.CompilerServices.Unsafe::As")]
		//[DllImport("System.Runtime.CompilerServices.Unsafe::As")]
		//public extern static ref TTo As<TFrom, TTo>(ref TFrom source);
		
	}
}
