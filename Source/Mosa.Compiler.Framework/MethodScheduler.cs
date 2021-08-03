// Copyright (c) MOSA Project. Licensed under the New BSD License.

using Mosa.Compiler.Framework.Trace;
using Mosa.Compiler.MosaTypeSystem;

using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Channels;

namespace Mosa.Compiler.Framework
{
	/// <summary>
	/// Schedules compilation of types/methods.
	/// </summary>
	public sealed class MethodScheduler
	{
		#region Data Members

		public Compiler Compiler;

		private HashSet<MosaMethod> methodSet = new HashSet<MosaMethod>();

		private readonly Channel<MosaMethod> _channel;
		private int totalMethods;
		private int totalQueued;

		#endregion Data Members

		#region Properties

		public int PassCount { get; }

		/// <summary>
		/// Gets the total methods.
		/// </summary>
		/// <value>
		/// The total methods.
		/// </value>
		public int TotalMethods => totalMethods;

		/// <summary>
		/// Gets the queued methods.
		/// </summary>
		/// <value>
		/// The queued methods.
		/// </value>
		public int TotalQueuedMethods => totalQueued;

		#endregion Properties

		public MethodScheduler(Compiler compiler)
		{
			_channel = Channel.CreateUnbounded<MosaMethod>(new UnboundedChannelOptions { SingleWriter = false, SingleReader = false });
			Compiler = compiler;
			PassCount = 0;
		}

		public void ScheduleAll(TypeSystem typeSystem)
		{
			foreach (var type in typeSystem.AllTypes)
			{
				Schedule(type);
			}
		}

		public bool IsCompilable(MosaType type)
		{
			if (type.IsModule)
				return false;

			if (type.IsInterface)
				return false;

			if (type.HasOpenGenericParams || type.IsPointer)
				return false;

			return true;
		}

		public bool IsCompilable(MosaMethod method)
		{
			if (method.IsAbstract && !method.HasImplementation)
				return false;

			if (method.HasOpenGenericParams)
				return false;

			if (method.IsCompilerGenerated)
				return false;

			return true;
		}

		public void Schedule(MosaType type)
		{
			if (!IsCompilable(type))
				return;

			foreach (var method in type.Methods)
			{
				Schedule(method);
			}
		}

		public void Schedule(MosaMethod method)
		{
			if (!IsCompilable(method))
				return;

			AddToQueue(method);
		}

		private void AddToQueue(MosaMethod method)
		{
			lock (methodSet)
			{
				if (!methodSet.Contains(method))
				{
					methodSet.Add(method);

					Interlocked.Increment(ref totalMethods);
				}
			}

			Interlocked.Increment(ref totalQueued);

			if (!_channel.Writer.TryWrite(method))
			{
				Debug.Assert(false);
			}
		}

		public MosaMethod GetMethodToCompile()
		{
			if (_channel.Reader.TryRead(out var result))
			{
				Interlocked.Decrement(ref totalQueued);
			};

			return result;
		}

		public void AddToRecompileQueue(HashSet<MosaMethod> methods)
		{
			foreach (var method in methods)
				AddToQueue(method);
		}

		public void AddToRecompileQueue(MosaMethod method)
		{
			AddToQueue(method);
		}
	}
}
