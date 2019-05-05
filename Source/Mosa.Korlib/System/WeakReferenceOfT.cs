// Copyright (c) MOSA Project. Licensed under the New BSD License.

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/*============================================================
**
**
** Purpose: A wrapper for establishing a WeakReference to a generic type.
**
===========================================================*/

using System;
using System.Runtime;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Threading;
using System.Diagnostics;

using Internal.Runtime.CompilerServices;

namespace System
{
	// This class is sealed to mitigate security issues caused by Object::MemberwiseClone.
	[Serializable]
	[System.Runtime.CompilerServices.TypeForwardedFrom("mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089")]
	public sealed class WeakReference<T> : ISerializable where T : class
	{
		// If you fix bugs here, please fix them in WeakReference at the same time.

		internal GCHandle m_handle;
		private bool m_trackResurrection;

		// Creates a new WeakReference that keeps track of target.
		// Assumes a Short Weak Reference (ie TrackResurrection is false.)
		//
		public WeakReference(T target)
			: this(target, false)
		{
		}

		//Creates a new WeakReference that keeps track of target.
		//
		public WeakReference(T target, bool trackResurrection)
		{
			m_handle = GCHandle.Alloc(target, trackResurrection ? GCHandleType.WeakTrackResurrection : GCHandleType.Weak);
			m_trackResurrection = trackResurrection;
		}

		internal WeakReference(SerializationInfo info, StreamingContext context)
		{
			if (info == null)
			{
				throw new ArgumentNullException(nameof(info));
			}

			T target = (T)info.GetValue("TrackedObject", typeof(T)); // Do not rename (binary serialization)
			bool trackResurrection = info.GetBoolean("TrackResurrection"); // Do not rename (binary serialization)

			m_handle = GCHandle.Alloc(target, trackResurrection ? GCHandleType.WeakTrackResurrection : GCHandleType.Weak);
		}

		//
		// We are exposing TryGetTarget instead of a simple getter to avoid a common problem where people write incorrect code like:
		//
		//      WeakReference ref = ...;
		//      if (ref.Target != null)
		//          DoSomething(ref.Target)
		//
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool TryGetTarget(out T target)
		{
			// Call the worker method that has more performant but less user friendly signature.
			T o = GetTarget();
			target = o;
			return o != null;
		}

		public void SetTarget(T target)
		{
			m_handle.Target = target;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private T GetTarget()
		{
			if (!m_handle.IsAllocated)
				return null;

			T target = Unsafe.As<T>(m_handle.Target);

			return target;
		}

		// Free all system resources associated with this reference.
		//
		// Note: The WeakReference<T> finalizer is not usually run, but
		// treated specially in gc.cpp's ScanForFinalization
		// This is needed for subclasses deriving from WeakReference<T>, however.
		// Additionally, there may be some cases during shutdown when we run this finalizer.
		~WeakReference()
		{
			m_handle.Free();
		}

		public void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			if (info == null)
			{
				throw new ArgumentNullException(nameof(info));
			}

			info.AddValue("TrackedObject", this.GetTarget(), typeof(T)); // Do not rename (binary serialization)
			info.AddValue("TrackResurrection", m_trackResurrection); // Do not rename (binary serialization)
		}
	}
}
