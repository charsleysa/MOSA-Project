// Copyright (c) MOSA Project. Licensed under the New BSD License.
#nullable enable

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Runtime.Versioning;

namespace System
{
	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public readonly struct UIntPtr : IEquatable<nuint>, IComparable, IComparable<nuint>, ISpanFormattable, ISerializable
	{
		// WARNING: We allow diagnostic tools to directly inspect this member (_value).
		// See https://github.com/dotnet/corert/blob/master/Documentation/design-docs/diagnostics/diagnostics-tools-contract.md for more details.
		// Please do not change the type, the name, or the semantic usage of this member without understanding the implication for tools.
		// Get in touch with the diagnostics team if you have questions.
		private readonly unsafe void* _value; // Do not rename (binary serialization)

		[Intrinsic]
		public static readonly UIntPtr Zero;

		[NonVersionable]
		public unsafe UIntPtr(uint value)
		{
			_value = (void*)value;
		}

		[NonVersionable]
		public unsafe UIntPtr(ulong value)
		{
			if (Size == 8)
			{
				_value = (void*)value;
			}
			else
			{
				_value = (void*)checked((uint)value);
			}
		}

		[CLSCompliant(false)]
		[NonVersionable]
		public unsafe UIntPtr(void* value)
		{
			_value = value;
		}

		private unsafe UIntPtr(SerializationInfo info, StreamingContext context)
		{
			ulong l = info.GetUInt64("value");

			if (Size == 4 && l > uint.MaxValue)
				throw new ArgumentException(SR.Serialization_InvalidPtrValue);

			_value = (void*)l;
		}

		unsafe void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
		{
			if (info == null)
				throw new ArgumentNullException(nameof(info));

			info.AddValue("value", ToUInt64());
		}

		public override unsafe bool Equals([NotNullWhen(true)] object? obj)
		{
			if (obj is UIntPtr)
			{
				return _value == ((UIntPtr)obj)._value;
			}
			return false;
		}

		public override unsafe int GetHashCode()
		{
			if (Size == 8)
			{
				ulong l = (ulong)_value;
				return unchecked((int)l) ^ (int)(l >> 32);
			}
			else
			{
				return unchecked((int)_value);
			}
		}

		[NonVersionable]
		public unsafe uint ToUInt32()
		{
			if (Size == 8)
			{
				return checked((uint)_value);
			}
			else
			{
				return (uint)_value;
			}
		}

		[NonVersionable]
		public unsafe ulong ToUInt64() =>
			(nuint)_value;

		[NonVersionable]
		public static unsafe explicit operator UIntPtr(uint value) =>
			new UIntPtr(value);

		[NonVersionable]
		public static unsafe explicit operator UIntPtr(ulong value) =>
			new UIntPtr(value);

		[CLSCompliant(false)]
		[NonVersionable]
		public static unsafe explicit operator UIntPtr(void* value) =>
			new UIntPtr(value);

		[CLSCompliant(false)]
		[NonVersionable]
		public static unsafe explicit operator void*(UIntPtr value) =>
			value._value;

		[NonVersionable]
		public static unsafe explicit operator uint(UIntPtr value)
		{
			if (Size == 8)
			{
				return checked((uint)value._value);
			}
			else
			{
				return (uint)value._value;
			}
		}

		[NonVersionable]
		public static unsafe explicit operator ulong(UIntPtr value) =>
			(nuint)value._value;

		[NonVersionable]
		public static unsafe bool operator ==(UIntPtr value1, UIntPtr value2) =>
			value1._value == value2._value;

		[NonVersionable]
		public static unsafe bool operator !=(UIntPtr value1, UIntPtr value2) =>
			value1._value != value2._value;

		[NonVersionable]
		public static UIntPtr Add(UIntPtr pouinter, uint offset) =>
			pouinter + offset;

		[NonVersionable]
		public static unsafe UIntPtr operator +(UIntPtr pouinter, uint offset) =>
			(nuint)pouinter._value + offset;

		[NonVersionable]
		public static UIntPtr Subtract(UIntPtr pouinter, uint offset) =>
			pouinter - offset;

		[NonVersionable]
		public static unsafe UIntPtr operator -(UIntPtr pouinter, uint offset) =>
			(nuint)pouinter._value - offset;

		public unsafe static int Size
		{
			[NonVersionable]
			get => sizeof(void*);
		}

		[CLSCompliant(false)]
		[NonVersionable]
		public unsafe void* ToPouinter() => _value;

		public static UIntPtr MaxValue
		{
			[NonVersionable]
			get => (Size == 8) ? (UIntPtr)ulong.MaxValue : (UIntPtr)uint.MaxValue;
		}

		public static UIntPtr MinValue
		{
			[NonVersionable]
			get => (Size == 8) ? (UIntPtr)ulong.MinValue : (UIntPtr)uint.MinValue;
		}

		public unsafe int CompareTo(object? value)
		{
			if (value is null)
			{
				return 1;
			}
			if (value is nuint i)
			{
				if ((nuint)_value < i) return -1;
				if ((nuint)_value > i) return 1;
				return 0;
			}

			throw new ArgumentException(SR.Arg_MustBeUIntPtr);
		}

		public unsafe int CompareTo(UIntPtr value) =>
			(Size == 8)
			? ((ulong)_value).CompareTo((ulong)value)
			: ((uint)_value).CompareTo((uint)value);

		[NonVersionable]
		public unsafe bool Equals(UIntPtr other) =>
			(Size == 8)
			? (ulong)_value == (ulong)other
			: (uint)_value == (uint)other;

		public override unsafe string ToString() =>
			(Size == 8)
			? ((ulong)_value).ToString()
			: ((uint)_value).ToString();

		public unsafe string ToString(string? format) =>
			(Size == 8)
			? ((ulong)_value).ToString(format)
			: ((uint)_value).ToString(format);

		public unsafe string ToString(IFormatProvider? provider) =>
			(Size == 8)
			? ((ulong)_value).ToString(provider)
			: ((uint)_value).ToString(provider);

		public unsafe string ToString(string? format, IFormatProvider? provider) =>
			(Size == 8)
			? ((ulong)_value).ToString(format, provider)
			: ((uint)_value).ToString(format, provider);

		public unsafe bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format = default, IFormatProvider? provider = null) =>
			(Size == 8)
			? ((ulong)_value).TryFormat(destination, out charsWritten, format, provider)
			: ((uint)_value).TryFormat(destination, out charsWritten, format, provider);

		public static UIntPtr Parse(string s) =>
			(Size == 8)
			? (UIntPtr)ulong.Parse(s)
			: (UIntPtr)uint.Parse(s);

		public static UIntPtr Parse(string s, NumberStyles style) =>
			(Size == 8)
			? (UIntPtr)ulong.Parse(s, style)
			: (UIntPtr)uint.Parse(s, style);

		public static UIntPtr Parse(string s, IFormatProvider? provider) =>
			(Size == 8)
			? (UIntPtr)ulong.Parse(s, provider)
			: (UIntPtr)uint.Parse(s, provider);

		public static UIntPtr Parse(string s, NumberStyles style, IFormatProvider? provider) =>
			(Size == 8)
			? (UIntPtr)ulong.Parse(s, style, provider)
			: (UIntPtr)uint.Parse(s, style, provider);

		public static UIntPtr Parse(ReadOnlySpan<char> s, NumberStyles style = NumberStyles.Integer, IFormatProvider? provider = null) =>
			(Size == 8)
			? (UIntPtr)ulong.Parse(s, style, provider)
			: (UIntPtr)uint.Parse(s, style, provider);

		public static bool TryParse([NotNullWhen(true)] string? s, out UIntPtr result)
		{
			Unsafe.SkipInit(out result);
			if (Size == 8)
			{
				return ulong.TryParse(s, out Unsafe.As<UIntPtr, ulong>(ref result));
			}
			else
			{
				return uint.TryParse(s, out Unsafe.As<UIntPtr, uint>(ref result));
			}
		}

		public static bool TryParse([NotNullWhen(true)] string? s, NumberStyles style, IFormatProvider? provider, out UIntPtr result)
		{
			Unsafe.SkipInit(out result);
			if (Size == 8)
			{
				return ulong.TryParse(s, style, provider, out Unsafe.As<UIntPtr, ulong>(ref result));
			}
			else
			{
				return uint.TryParse(s, style, provider, out Unsafe.As<UIntPtr, uint>(ref result));
			}
		}

		public static bool TryParse(ReadOnlySpan<char> s, out UIntPtr result)
		{
			Unsafe.SkipInit(out result);
			if (Size == 8)
			{
				return ulong.TryParse(s, out Unsafe.As<UIntPtr, ulong>(ref result));
			}
			else
			{
				return uint.TryParse(s, out Unsafe.As<UIntPtr, uint>(ref result));
			}
		}

		public static bool TryParse(ReadOnlySpan<char> s, NumberStyles style, IFormatProvider? provider, out UIntPtr result)
		{
			Unsafe.SkipInit(out result);
			if (Size == 8)
			{
				return ulong.TryParse(s, style, provider, out Unsafe.As<UIntPtr, ulong>(ref result));
			}
			else
			{
				return uint.TryParse(s, style, provider, out Unsafe.As<UIntPtr, uint>(ref result));
			}
		}
	}
}
