// Copyright (c) MOSA Project. Licensed under the New BSD License.

using System.Runtime;
using System.Threading;
using System.Collections;
using System.Diagnostics;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;

using Internal.Runtime.CompilerServices;

namespace System
{
	/// <summary>
	/// Array
	/// </summary>
	public abstract partial class Array : ICloneable, IList, ICollection, IEnumerable, IStructuralComparable, IStructuralEquatable
	{
		private int length;

		public int Length
		{
			get
			{
				return length;
			}
		}

		/// <summary>
		/// Gets the rank (number of dimensions) of the Array. For example, a one-dimensional array returns 1, a two-dimensional array returns 2, and so on.
		/// </summary>
		public int Rank
		{
			// TODO: support multidimensional arrays
			get { return 1; }
		}

		/// <summary>
		/// SetValue
		/// </summary>
		public void SetValue(object value, params int[] indices)
		{
			if (indices == null)
				throw new ArgumentNullException(nameof(indices));
			if (Rank != indices.Length)
				throw new ArgumentException("The number of dimensions in the current Array is not equal to the number of elements in indices.", nameof(indices));

			// TODO
		}

		/// <summary>
		/// GetValue
		/// </summary>
		public object GetValue(params int[] indices)
		{
			if (indices == null)
				throw new ArgumentNullException(nameof(indices));
			if (Rank != indices.Length)
				throw new ArgumentException("The number of dimensions in the current Array is not equal to the number of elements in indices.", nameof(indices));

			// TODO
			return null;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Copy(Array sourceArray, int sourceIndex, Array destinationArray, int destinationIndex, int length, bool reliable);

		/// <summary>
		/// Copies a range of elements from an Array starting at the specified source index and pastes them to another Array starting at the specified destination index.
		/// The length and the indexes are specified as 32-bit integers.
		/// </summary>
		public static void Copy(Array sourceArray, int sourceIndex, Array destinationArray, int destinationIndex, int length)
		{
			Copy(sourceArray, sourceIndex, destinationArray, destinationIndex, length, true);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern int GetLength(int dimension);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern int GetLowerBound(int dimension);

		public int GetUpperBound(int dimension)
		{
			return GetLowerBound(dimension) + GetLength(dimension) - 1;
		}
	}

	internal class ArrayEnumeratorBase
	{
		protected int _index;
		protected int _endIndex;

		internal ArrayEnumeratorBase()
		{
			_index = -1;
		}

		public bool MoveNext()
		{
			if (_index < _endIndex)
			{
				_index++;
				return (_index < _endIndex);
			}
			return false;
		}

		public void Dispose()
		{
		}
	}

	//
	// Note: the declared base type and interface list also determines what Reflection returns from TypeInfo.BaseType and TypeInfo.ImplementedInterfaces for array types.
	// This also means the class must be declared "public" so that the framework can reflect on it.
	//
	public class Array<T> : Array, IEnumerable<T>, ICollection<T>, IList<T>, IReadOnlyList<T>
	{
		public new IEnumerator<T> GetEnumerator()
		{
			// get length so we don't have to call the Length property again in ArrayEnumerator constructor
			// and avoid more checking there too.
			int length = this.Length;
			return length == 0 ? ArrayEnumerator.Empty : new ArrayEnumerator(Unsafe.As<T[]>(this), length);
		}

		public int Count
		{
			get
			{
				return this.Length;
			}
		}

		//
		// Fun fact:
		//
		//  ((int[])a).IsReadOnly returns false.
		//  ((IList<int>)a).IsReadOnly returns true.
		//
		public new bool IsReadOnly
		{
			get
			{
				return true;
			}
		}

		public void Add(T item)
		{
			throw new NotSupportedException();
		}

		public void Clear()
		{
			throw new NotSupportedException();
		}

		public bool Contains(T item)
		{
			T[] array = Unsafe.As<T[]>(this);
			return Array.IndexOf(array, item, 0, array.Length) >= 0;
		}

		public void CopyTo(T[] array, int arrayIndex)
		{
			Array.Copy(Unsafe.As<T[]>(this), 0, array, arrayIndex, this.Length);
		}

		public bool Remove(T item)
		{
			throw new NotSupportedException();
		}

		public T this[int index]
		{
			get
			{
				try
				{
					return Unsafe.As<T[]>(this)[index];
				}
				catch (IndexOutOfRangeException)
				{
					throw new ArgumentOutOfRangeException(nameof(index), SR.ArgumentOutOfRange_Index);
				}
			}
			set
			{
				try
				{
					Unsafe.As<T[]>(this)[index] = value;
				}
				catch (IndexOutOfRangeException)
				{
					throw new ArgumentOutOfRangeException(nameof(index), SR.ArgumentOutOfRange_Index);
				}
			}
		}

		public int IndexOf(T item)
		{
			T[] array = Unsafe.As<T[]>(this);
			return Array.IndexOf(array, item, 0, array.Length);
		}

		public void Insert(int index, T item)
		{
			throw new NotSupportedException();
		}

		public void RemoveAt(int index)
		{
			throw new NotSupportedException();
		}

		private sealed class ArrayEnumerator : ArrayEnumeratorBase, IEnumerator<T>, ICloneable
		{
			private T[] _array;

			// Passing -1 for endIndex so that MoveNext always returns false without mutating _index
			internal static readonly ArrayEnumerator Empty = new ArrayEnumerator(null, -1);

			internal ArrayEnumerator(T[] array, int endIndex)
			{
				_array = array;
				_endIndex = endIndex;
			}

			public T Current
			{
				get
				{
					if (_index < 0 || _index >= _endIndex)
						throw new InvalidOperationException();
					return _array[_index];
				}
			}

			object IEnumerator.Current
			{
				get
				{
					return Current;
				}
			}

			void IEnumerator.Reset()
			{
				_index = -1;
			}

			public object Clone()
			{
				return MemberwiseClone();
			}
		}
	}
}
