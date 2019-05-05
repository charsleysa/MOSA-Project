// Copyright (c) MOSA Project. Licensed under the New BSD License.

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/*============================================================
**
**
** Purpose: A wrapper for establishing a WeakReference to an Object.
**
===========================================================*/

using System;
using System.Runtime;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Threading;
using System.Diagnostics;

namespace System
{
	[Serializable]
	[System.Runtime.CompilerServices.TypeForwardedFrom("mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089")]
	public class WeakReference : ISerializable
	{
		// If you fix bugs here, please fix them in WeakReference<T> at the same time.

		// Most methods using m_handle should use GC.KeepAlive(this) to avoid potential handle recycling
		// attacks (i.e. if the WeakReference instance is finalized away underneath you when you're still
		// handling a cached value of the handle then the handle could be freed and reused).
		internal GCHandle m_handle;

		internal bool m_IsLongReference;

		// Creates a new WeakReference that keeps track of target.
		// Assumes a Short Weak Reference (ie TrackResurrection is false.)
		//
		public WeakReference(object target)
			: this(target, false)
		{
		}

		//Creates a new WeakReference that keeps track of target.
		//
		public WeakReference(object target, bool trackResurrection)
		{
			m_IsLongReference = trackResurrection;
			m_handle = GCHandle.Alloc(target, trackResurrection ? GCHandleType.WeakTrackResurrection : GCHandleType.Weak);
		}

		protected WeakReference(SerializationInfo info, StreamingContext context)
		{
			if (info == null)
			{
				throw new ArgumentNullException(nameof(info));
			}

			object target = info.GetValue("TrackedObject", typeof(object)); // Do not rename (binary serialization)
			bool trackResurrection = info.GetBoolean("TrackResurrection"); // Do not rename (binary serialization)

			m_IsLongReference = trackResurrection;
			m_handle = GCHandle.Alloc(target, trackResurrection ? GCHandleType.WeakTrackResurrection : GCHandleType.Weak);
		}

		//Determines whether or not this instance of WeakReference still refers to an object
		//that has not been collected.
		//
		public virtual bool IsAlive
		{
			get
			{
				return (Target != null);
			}
		}

		//Returns a boolean indicating whether or not we're tracking objects until they're collected (true)
		//or just until they're finalized (false).
		//
		public virtual bool TrackResurrection
		{
			get { return m_IsLongReference; }
		}

		//Gets the Object stored in the handle if it's accessible.
		// Or sets it.
		//
		public virtual object Target
		{
			get
			{
				if (!m_handle.IsAllocated)
					return null;
				return m_handle.Target;
			}

			set
			{
				m_handle.Target = value;
			}
		}

		public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			if (info == null)
			{
				throw new ArgumentNullException(nameof(info));
			}

			info.AddValue("TrackedObject", Target, typeof(object)); // Do not rename (binary serialization)
			info.AddValue("TrackResurrection", m_IsLongReference); // Do not rename (binary serialization)
		}

		// Free all system resources associated with this reference.
		~WeakReference()
		{
			m_handle.Free();
		}
	}
}
