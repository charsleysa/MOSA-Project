// Copyright (c) MOSA Project. Licensed under the New BSD License.
#nullable enable

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Numerics;
using System.Reflection;
using System.Resources;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace System.Diagnostics.Tracing
{
	/// <summary>
	/// This class is meant to be inherited by a user-defined event source in order to define a managed
	/// ETW provider.   Please See DESIGN NOTES above for the internal architecture.
	/// The minimal definition of an EventSource simply specifies a number of ETW event methods that
	/// call one of the EventSource.WriteEvent overloads, <see cref="EventSource.WriteEventCore"/>,
	/// or <see cref="EventSource.WriteEventWithRelatedActivityIdCore"/> to log them. This functionality
	/// is sufficient for many users.
	/// <para>
	/// To achieve more control over the ETW provider manifest exposed by the event source type, the
	/// [<see cref="EventAttribute"/>] attributes can be specified for the ETW event methods.
	/// </para><para>
	/// For very advanced EventSources, it is possible to intercept the commands being given to the
	/// eventSource and change what filtering is done (see EventListener.EnableEvents and
	/// <see cref="EventListener.DisableEvents"/>) or cause actions to be performed by the eventSource,
	/// e.g. dumping a data structure (see EventSource.SendCommand and
	/// <see cref="EventSource.OnEventCommand"/>).
	/// </para><para>
	/// The eventSources can be turned on with Windows ETW controllers (e.g. logman), immediately.
	/// It is also possible to control and intercept the data dispatcher programmatically.  See
	/// <see cref="EventListener"/> for more.
	/// </para>
	/// </summary>
	/// <remarks>
	/// This is a minimal definition for a custom event source:
	/// <code>
	/// [EventSource(Name="Samples.Demos.Minimal")]
	/// sealed class MinimalEventSource : EventSource
	/// {
	///     public static MinimalEventSource Log = new MinimalEventSource();
	///     public void Load(long ImageBase, string Name) { WriteEvent(1, ImageBase, Name); }
	///     public void Unload(long ImageBase) { WriteEvent(2, ImageBase); }
	///     private MinimalEventSource() {}
	/// }
	/// </code>
	/// </remarks>

	// The EnsureDescriptorsInitialized() method might need to access EventSource and its derived type
	// members and the trimmer ensures that these members are preserved.
	[DynamicallyAccessedMembers(ManifestMemberTypes)]
	[UnconditionalSuppressMessage("ReflectionAnalysis", "IL2113:ReflectionToRequiresUnreferencedCode",
		Justification = "EnsureDescriptorsInitialized's use of GetType preserves methods on Delegate and MulticastDelegate " +
						"because the nested type OverrideEventProvider's base type EventProvider defines a delegate. " +
						"This includes Delegate and MulticastDelegate methods which require unreferenced code, but " +
						"EnsureDescriptorsInitialized does not access these members and is safe to call.")]
	[UnconditionalSuppressMessage("ReflectionAnalysis", "IL2115:ReflectionToDynamicallyAccessedMembers",
		Justification = "EnsureDescriptorsInitialized's use of GetType preserves methods on Delegate and MulticastDelegate " +
						"because the nested type OverrideEventProvider's base type EventProvider defines a delegate. " +
						"This includes Delegate and MulticastDelegate methods which have dynamically accessed members requirements, but " +
						"EnsureDescriptorsInitialized does not access these members and is safe to call.")]
	public partial class EventSource : IDisposable
	{
		internal static bool IsSupported { get; } = false;

		/// <summary>
		/// The human-friendly name of the eventSource.  It defaults to the simple name of the class
		/// </summary>
		public string Name => throw new PlatformNotSupportedException();

		/// <summary>
		/// Every eventSource is assigned a GUID to uniquely identify it to the system.
		/// </summary>
		public Guid Guid => throw new PlatformNotSupportedException();

		/// <summary>
		/// Returns true if the eventSource has been enabled at all. This is the preferred test
		/// to be performed before a relatively expensive EventSource operation.
		/// </summary>
		public bool IsEnabled()
		{
			return false;
		}

		/// <summary>
		/// Returns true if events with greater than or equal 'level' and have one of 'keywords' set are enabled.
		///
		/// Note that the result of this function is only an approximation on whether a particular
		/// event is active or not. It is only meant to be used as way of avoiding expensive
		/// computation for logging when logging is not on, therefore it sometimes returns false
		/// positives (but is always accurate when returning false).  EventSources are free to
		/// have additional filtering.
		/// </summary>
		public bool IsEnabled(EventLevel level, EventKeywords keywords)
		{
			return false;
		}

		/// <summary>
		/// Returns true if events with greater than or equal 'level' and have one of 'keywords' set are enabled, or
		/// if 'keywords' specifies a channel bit for a channel that is enabled.
		///
		/// Note that the result of this function only an approximation on whether a particular
		/// event is active or not. It is only meant to be used as way of avoiding expensive
		/// computation for logging when logging is not on, therefore it sometimes returns false
		/// positives (but is always accurate when returning false).  EventSources are free to
		/// have additional filtering.
		/// </summary>
		public bool IsEnabled(EventLevel level, EventKeywords keywords, EventChannel channel)
		{
			return false;
		}

		/// <summary>
		/// Returns the settings for the event source instance
		/// </summary>
		public EventSourceSettings Settings => throw new PlatformNotSupportedException();

		// Manifest support
		/// <summary>
		/// Returns the GUID that uniquely identifies the eventSource defined by 'eventSourceType'.
		/// This API allows you to compute this without actually creating an instance of the EventSource.
		/// It only needs to reflect over the type.
		/// </summary>
		public static Guid GetGuid(Type eventSourceType)
		{
			throw new PlatformNotSupportedException();
		}

		/// <summary>
		/// Returns the official ETW Provider name for the eventSource defined by 'eventSourceType'.
		/// This API allows you to compute this without actually creating an instance of the EventSource.
		/// It only needs to reflect over the type.
		/// </summary>
		public static string GetName(Type eventSourceType)
		{
			throw new PlatformNotSupportedException();
		}

		private const DynamicallyAccessedMemberTypes ManifestMemberTypes = DynamicallyAccessedMemberTypes.All;

		/// <summary>
		/// Returns a string of the XML manifest associated with the eventSourceType. The scheme for this XML is
		/// documented at in EventManifest Schema https://docs.microsoft.com/en-us/windows/desktop/WES/eventmanifestschema-schema.
		/// This is the preferred way of generating a manifest to be embedded in the ETW stream as it is fast and
		/// the fact that it only includes localized entries for the current UI culture is an acceptable tradeoff.
		/// </summary>
		/// <param name="eventSourceType">The type of the event source class for which the manifest is generated</param>
		/// <param name="assemblyPathToIncludeInManifest">The manifest XML fragment contains the string name of the DLL name in
		/// which it is embedded.  This parameter specifies what name will be used</param>
		/// <returns>The XML data string</returns>
		[UnconditionalSuppressMessage("ReflectionAnalysis", "IL2114:ReflectionToDynamicallyAccessedMembers",
			Justification = "EnsureDescriptorsInitialized's use of GetType preserves this method which " +
							"has dynamically accessed members requirements, but EnsureDescriptorsInitialized does not " +
							"access this member and is safe to call.")]
		public static string? GenerateManifest(
			[DynamicallyAccessedMembers(ManifestMemberTypes)]
			Type eventSourceType,
			string? assemblyPathToIncludeInManifest)
		{
			throw new PlatformNotSupportedException();
		}

		/// <summary>
		/// Returns a string of the XML manifest associated with the eventSourceType. The scheme for this XML is
		/// documented at in EventManifest Schema https://docs.microsoft.com/en-us/windows/desktop/WES/eventmanifestschema-schema.
		/// Pass EventManifestOptions.AllCultures when generating a manifest to be registered on the machine. This
		/// ensures that the entries in the event log will be "optimally" localized.
		/// </summary>
		/// <param name="eventSourceType">The type of the event source class for which the manifest is generated</param>
		/// <param name="assemblyPathToIncludeInManifest">The manifest XML fragment contains the string name of the DLL name in
		/// which it is embedded.  This parameter specifies what name will be used</param>
		/// <param name="flags">The flags to customize manifest generation. If flags has bit OnlyIfNeededForRegistration specified
		/// this returns null when the eventSourceType does not require explicit registration</param>
		/// <returns>The XML data string or null</returns>
		[UnconditionalSuppressMessage("ReflectionAnalysis", "IL2114:ReflectionToDynamicallyAccessedMembers",
			Justification = "EnsureDescriptorsInitialized's use of GetType preserves this method which " +
							"has dynamically accessed members requirements, but EnsureDescriptorsInitialized does not " +
							"access this member and is safe to call.")]
		public static string? GenerateManifest(
			[DynamicallyAccessedMembers(ManifestMemberTypes)]
			Type eventSourceType,
			string? assemblyPathToIncludeInManifest,
			EventManifestOptions flags)
		{
			return null;
		}

		// EventListener support
		/// <summary>
		/// returns a list (IEnumerable) of all sources in the appdomain).  EventListeners typically need this.
		/// </summary>
		/// <returns></returns>
		public static IEnumerable<EventSource> GetSources()
		{
			return Array.Empty<EventSource>();
		}

		/// <summary>
		/// Send a command to a particular EventSource identified by 'eventSource'.
		/// Calling this routine simply forwards the command to the EventSource.OnEventCommand
		/// callback.  What the EventSource does with the command and its arguments are from
		/// that point EventSource-specific.
		/// </summary>
		/// <param name="eventSource">The instance of EventSource to send the command to</param>
		/// <param name="command">A positive user-defined EventCommand, or EventCommand.SendManifest</param>
		/// <param name="commandArguments">A set of (name-argument, value-argument) pairs associated with the command</param>
		public static void SendCommand(EventSource eventSource, EventCommand command, IDictionary<string, string?>? commandArguments)
		{
			return;
		}

		// Error APIs.  (We don't throw by default, but you can probe for status)
		/// <summary>
		/// Because
		///
		///     1) Logging is often optional and thus should not generate fatal errors (exceptions)
		///     2) EventSources are often initialized in class constructors (which propagate exceptions poorly)
		///
		/// The event source constructor does not throw exceptions.  Instead we remember any exception that
		/// was generated (it is also logged to Trace.WriteLine).
		/// </summary>
		public Exception? ConstructionException => null;

		/// <summary>
		/// EventSources can have arbitrary string key-value pairs associated with them called Traits.
		/// These traits are not interpreted by the EventSource but may be interpreted by EventListeners
		/// (e.g. like the built in ETW listener).   These traits are specified at EventSource
		/// construction time and can be retrieved by using this GetTrait API.
		/// </summary>
		/// <param name="key">The key to look up in the set of key-value pairs passed to the EventSource constructor</param>
		/// <returns>The value string associated with key.  Will return null if there is no such key.</returns>
		public string? GetTrait(string key)
		{
			return null;
		}

		/// <summary>
		/// Displays the name and GUID for the eventSource for debugging purposes.
		/// </summary>
		public override string ToString()
		{
			return base.ToString()!;
		}

		/// <summary>
		/// Fires when a Command (e.g. Enable) comes from a an EventListener.
		/// </summary>
		public event EventHandler<EventCommandEventArgs>? EventCommandExecuted
		{
			add
			{
				if (value == null)
					return;

				m_eventCommandExecuted += value;

				// If we have an EventHandler<EventCommandEventArgs> attached to the EventSource before the first command arrives
				// It should get a chance to handle the deferred commands.
				EventCommandEventArgs? deferredCommands = m_deferredCommands;
				while (deferredCommands != null)
				{
					value(this, deferredCommands);
					deferredCommands = deferredCommands.nextCommand;
				}
			}
			remove
			{
				m_eventCommandExecuted -= value;
			}
		}

		#region ActivityID

		/// <summary>
		/// When a thread starts work that is on behalf of 'something else' (typically another
		/// thread or network request) it should mark the thread as working on that other work.
		/// This API marks the current thread as working on activity 'activityID'. This API
		/// should be used when the caller knows the thread's current activity (the one being
		/// overwritten) has completed. Otherwise, callers should prefer the overload that
		/// return the oldActivityThatWillContinue (below).
		///
		/// All events created with the EventSource on this thread are also tagged with the
		/// activity ID of the thread.
		///
		/// It is common, and good practice after setting the thread to an activity to log an event
		/// with a 'start' opcode to indicate that precise time/thread where the new activity
		/// started.
		/// </summary>
		/// <param name="activityId">A Guid that represents the new activity with which to mark
		/// the current thread</param>
		public static void SetCurrentThreadActivityId(Guid activityId)
		{
			return;
		}

		/// <summary>
		/// Retrieves the ETW activity ID associated with the current thread.
		/// </summary>
		public static Guid CurrentThreadActivityId
		{
			get
			{
				return default;
			}
		}

		/// <summary>
		/// When a thread starts work that is on behalf of 'something else' (typically another
		/// thread or network request) it should mark the thread as working on that other work.
		/// This API marks the current thread as working on activity 'activityID'. It returns
		/// whatever activity the thread was previously marked with. There is a convention that
		/// callers can assume that callees restore this activity mark before the callee returns.
		/// To encourage this, this API returns the old activity, so that it can be restored later.
		///
		/// All events created with the EventSource on this thread are also tagged with the
		/// activity ID of the thread.
		///
		/// It is common, and good practice after setting the thread to an activity to log an event
		/// with a 'start' opcode to indicate that precise time/thread where the new activity
		/// started.
		/// </summary>
		/// <param name="activityId">A Guid that represents the new activity with which to mark
		/// the current thread</param>
		/// <param name="oldActivityThatWillContinue">The Guid that represents the current activity
		/// which will continue at some point in the future, on the current thread</param>
		public static void SetCurrentThreadActivityId(Guid activityId, out Guid oldActivityThatWillContinue)
		{
			oldActivityThatWillContinue = default;
		}

		#endregion ActivityID

		#region protected

		/// <summary>
		/// This is the constructor that most users will use to create their eventSource.   It takes
		/// no parameters.  The ETW provider name and GUID of the EventSource are determined by the EventSource
		/// custom attribute (so you can determine these things declaratively).   If the GUID for the eventSource
		/// is not specified in the EventSourceAttribute (recommended), it is Generated by hashing the name.
		/// If the ETW provider name of the EventSource is not given, the name of the EventSource class is used as
		/// the ETW provider name.
		/// </summary>
		protected EventSource()
			: this(EventSourceSettings.EtwManifestEventFormat)
		{ }

		/// <summary>
		/// By default calling the 'WriteEvent' methods do NOT throw on errors (they silently discard the event).
		/// This is because in most cases users assume logging is not 'precious' and do NOT wish to have logging failures
		/// crash the program. However for those applications where logging is 'precious' and if it fails the caller
		/// wishes to react, setting 'throwOnEventWriteErrors' will cause an exception to be thrown if WriteEvent
		/// fails. Note the fact that EventWrite succeeds does not necessarily mean that the event reached its destination
		/// only that operation of writing it did not fail. These EventSources will not generate self-describing ETW events.
		///
		/// For compatibility only use the EventSourceSettings.ThrowOnEventWriteErrors flag instead.
		/// </summary>
		// [Obsolete("Use the EventSource(EventSourceSettings) overload")]
		protected EventSource(bool throwOnEventWriteErrors)
			: this(EventSourceSettings.EtwManifestEventFormat | (throwOnEventWriteErrors ? EventSourceSettings.ThrowOnEventWriteErrors : 0))
		{ }

		/// <summary>
		/// Construct an EventSource with additional non-default settings (see EventSourceSettings for more)
		/// </summary>
		protected EventSource(EventSourceSettings settings) : this(settings, null) { }

		/// <summary>
		/// Construct an EventSource with additional non-default settings.
		///
		/// Also specify a list of key-value pairs called traits (you must pass an even number of strings).
		/// The first string is the key and the second is the value.   These are not interpreted by EventSource
		/// itself but may be interpreted the listeners.  Can be fetched with GetTrait(string).
		/// </summary>
		/// <param name="settings">See EventSourceSettings for more.</param>
		/// <param name="traits">A collection of key-value strings (must be an even number).</param>
		protected EventSource(EventSourceSettings settings, params string[]? traits)
		{
			throw new PlatformNotSupportedException();
		}

		/// <summary>
		/// This method is called when the eventSource is updated by the controller.
		/// </summary>
		protected virtual void OnEventCommand(EventCommandEventArgs command) { }

#pragma warning disable 1591

		// optimized for common signatures (no args)
		[UnconditionalSuppressMessage("ReflectionAnalysis", "IL2026:UnrecognizedReflectionPattern",
				   Justification = EventSourceSuppressMessage)]
		protected unsafe void WriteEvent(int eventId)
		{
			if (IsEnabled())
			{
				throw new PlatformNotSupportedException();
			}
		}

		// optimized for common signatures (ints)
		[UnconditionalSuppressMessage("ReflectionAnalysis", "IL2026:UnrecognizedReflectionPattern",
				   Justification = EventSourceSuppressMessage)]
		protected unsafe void WriteEvent(int eventId, int arg1)
		{
			if (IsEnabled())
			{
				throw new PlatformNotSupportedException();
			}
		}

		[UnconditionalSuppressMessage("ReflectionAnalysis", "IL2026:UnrecognizedReflectionPattern",
				   Justification = EventSourceSuppressMessage)]
		protected unsafe void WriteEvent(int eventId, int arg1, int arg2)
		{
			if (IsEnabled())
			{
				throw new PlatformNotSupportedException();
			}
		}

		[UnconditionalSuppressMessage("ReflectionAnalysis", "IL2026:UnrecognizedReflectionPattern",
				   Justification = EventSourceSuppressMessage)]
		protected unsafe void WriteEvent(int eventId, int arg1, int arg2, int arg3)
		{
			if (IsEnabled())
			{
				throw new PlatformNotSupportedException();
			}
		}

		// optimized for common signatures (longs)
		[UnconditionalSuppressMessage("ReflectionAnalysis", "IL2026:UnrecognizedReflectionPattern",
				   Justification = EventSourceSuppressMessage)]
		protected unsafe void WriteEvent(int eventId, long arg1)
		{
			if (IsEnabled())
			{
				throw new PlatformNotSupportedException();
			}
		}

		[UnconditionalSuppressMessage("ReflectionAnalysis", "IL2026:UnrecognizedReflectionPattern",
				   Justification = EventSourceSuppressMessage)]
		protected unsafe void WriteEvent(int eventId, long arg1, long arg2)
		{
			if (IsEnabled())
			{
				throw new PlatformNotSupportedException();
			}
		}

		[UnconditionalSuppressMessage("ReflectionAnalysis", "IL2026:UnrecognizedReflectionPattern",
				   Justification = EventSourceSuppressMessage)]
		protected unsafe void WriteEvent(int eventId, long arg1, long arg2, long arg3)
		{
			if (IsEnabled())
			{
				throw new PlatformNotSupportedException();
			}
		}

		// optimized for common signatures (strings)
		[UnconditionalSuppressMessage("ReflectionAnalysis", "IL2026:UnrecognizedReflectionPattern",
				   Justification = EventSourceSuppressMessage)]
		protected unsafe void WriteEvent(int eventId, string? arg1)
		{
			if (IsEnabled())
			{
				throw new PlatformNotSupportedException();
			}
		}

		[UnconditionalSuppressMessage("ReflectionAnalysis", "IL2026:UnrecognizedReflectionPattern",
				   Justification = EventSourceSuppressMessage)]
		protected unsafe void WriteEvent(int eventId, string? arg1, string? arg2)
		{
			if (IsEnabled())
			{
				throw new PlatformNotSupportedException();
			}
		}

		[UnconditionalSuppressMessage("ReflectionAnalysis", "IL2026:UnrecognizedReflectionPattern",
				   Justification = EventSourceSuppressMessage)]
		protected unsafe void WriteEvent(int eventId, string? arg1, string? arg2, string? arg3)
		{
			if (IsEnabled())
			{
				throw new PlatformNotSupportedException();
			}
		}

		// optimized for common signatures (string and ints)
		[UnconditionalSuppressMessage("ReflectionAnalysis", "IL2026:UnrecognizedReflectionPattern",
				   Justification = EventSourceSuppressMessage)]
		protected unsafe void WriteEvent(int eventId, string? arg1, int arg2)
		{
			if (IsEnabled())
			{
				throw new PlatformNotSupportedException();
			}
		}

		[UnconditionalSuppressMessage("ReflectionAnalysis", "IL2026:UnrecognizedReflectionPattern",
				   Justification = EventSourceSuppressMessage)]
		protected unsafe void WriteEvent(int eventId, string? arg1, int arg2, int arg3)
		{
			if (IsEnabled())
			{
				throw new PlatformNotSupportedException();
			}
		}

		// optimized for common signatures (string and longs)
		[UnconditionalSuppressMessage("ReflectionAnalysis", "IL2026:UnrecognizedReflectionPattern",
				   Justification = EventSourceSuppressMessage)]
		protected unsafe void WriteEvent(int eventId, string? arg1, long arg2)
		{
			if (IsEnabled())
			{
				throw new PlatformNotSupportedException();
			}
		}

		// optimized for common signatures (long and string)
		[UnconditionalSuppressMessage("ReflectionAnalysis", "IL2026:UnrecognizedReflectionPattern",
				   Justification = EventSourceSuppressMessage)]
		protected unsafe void WriteEvent(int eventId, long arg1, string? arg2)
		{
			if (IsEnabled())
			{
				throw new PlatformNotSupportedException();
			}
		}

		// optimized for common signatures (int and string)
		[UnconditionalSuppressMessage("ReflectionAnalysis", "IL2026:UnrecognizedReflectionPattern",
				   Justification = EventSourceSuppressMessage)]
		protected unsafe void WriteEvent(int eventId, int arg1, string? arg2)
		{
			if (IsEnabled())
			{
				throw new PlatformNotSupportedException();
			}
		}

		[UnconditionalSuppressMessage("ReflectionAnalysis", "IL2026:UnrecognizedReflectionPattern",
				   Justification = EventSourceSuppressMessage)]
		protected unsafe void WriteEvent(int eventId, byte[]? arg1)
		{
			if (IsEnabled())
			{
				throw new PlatformNotSupportedException();
			}
		}

		[UnconditionalSuppressMessage("ReflectionAnalysis", "IL2026:UnrecognizedReflectionPattern",
				   Justification = EventSourceSuppressMessage)]
		protected unsafe void WriteEvent(int eventId, long arg1, byte[]? arg2)
		{
			if (IsEnabled())
			{
				throw new PlatformNotSupportedException();
			}
		}

#pragma warning restore 1591

		/// <summary>
		/// Used to construct the data structure to be passed to the native ETW APIs - EventWrite and EventWriteTransfer.
		/// </summary>
		protected internal struct EventData
		{
			/// <summary>
			/// Address where the one argument lives (if this points to managed memory you must ensure the
			/// managed object is pinned.
			/// </summary>
			public unsafe IntPtr DataPointer
			{
				get => (IntPtr)(void*)m_Ptr;
				set => m_Ptr = unchecked((ulong)(void*)value);
			}

			/// <summary>
			/// Size of the argument referenced by DataPointer
			/// </summary>
			public int Size
			{
				get => m_Size;
				set => m_Size = value;
			}

			/// <summary>
			/// Reserved by ETW.  This property is present to ensure that we can zero it
			/// since System.Private.CoreLib uses are not zero'd.
			/// </summary>
			internal int Reserved
			{
				get => m_Reserved;
				set => m_Reserved = value;
			}

			#region private

			/// <summary>
			/// Initializes the members of this EventData object to point at a previously-pinned
			/// tracelogging-compatible metadata blob.
			/// </summary>
			/// <param name="pointer">Pinned tracelogging-compatible metadata blob.</param>
			/// <param name="size">The size of the metadata blob.</param>
			/// <param name="reserved">Value for reserved: 2 for per-provider metadata, 1 for per-event metadata</param>
			internal unsafe void SetMetadata(byte* pointer, int size, int reserved)
			{
				this.m_Ptr = (ulong)pointer;
				this.m_Size = size;
				this.m_Reserved = reserved; // Mark this descriptor as containing tracelogging-compatible metadata.
			}

			// Important, we pass this structure directly to the Win32 EventWrite API, so this structure must
			// be layed out exactly the way EventWrite wants it.
			internal ulong m_Ptr;

			internal int m_Size;
#pragma warning disable 0649
			internal int m_Reserved;       // Used to pad the size to match the Win32 API
#pragma warning restore 0649

			#endregion private
		}

		/// <summary>
		/// This routine allows you to create efficient WriteEvent helpers, however the code that you use to
		/// do this, while straightforward, is unsafe.
		/// </summary>
		/// <remarks>
		/// <code>
		///    protected unsafe void WriteEvent(int eventId, string arg1, long arg2)
		///    {
		///        if (IsEnabled())
		///        {
		///            arg2 ??= "";
		///            fixed (char* string2Bytes = arg2)
		///            {
		///                EventSource.EventData* descrs = stackalloc EventSource.EventData[2];
		///                descrs[0].DataPointer = (IntPtr)(&amp;arg1);
		///                descrs[0].Size = 8;
		///                descrs[0].Reserved = 0;
		///                descrs[1].DataPointer = (IntPtr)string2Bytes;
		///                descrs[1].Size = ((arg2.Length + 1) * 2);
		///                descrs[1].Reserved = 0;
		///                WriteEventCore(eventId, 2, descrs);
		///            }
		///        }
		///    }
		/// </code>
		/// </remarks>
		[UnconditionalSuppressMessage("ReflectionAnalysis", "IL2112:ReflectionToRequiresUnreferencedCode",
					Justification = "EnsureDescriptorsInitialized's use of GetType preserves this method which " +
									"requires unreferenced code, but EnsureDescriptorsInitialized does not access this member and is safe to call.")]
		[RequiresUnreferencedCode(EventSourceRequiresUnreferenceMessage)]
		[CLSCompliant(false)]
		protected unsafe void WriteEventCore(int eventId, int eventDataCount, EventSource.EventData* data)
		{
			WriteEventWithRelatedActivityIdCore(eventId, null, eventDataCount, data);
		}

		/// <summary>
		/// This routine allows you to create efficient WriteEventWithRelatedActivityId helpers, however the code
		/// that you use to do this, while straightforward, is unsafe. The only difference from
		/// <see cref="WriteEventCore"/> is that you pass the relatedActivityId from caller through to this API
		/// </summary>
		/// <remarks>
		/// <code>
		///    protected unsafe void WriteEventWithRelatedActivityId(int eventId, Guid relatedActivityId, string arg1, long arg2)
		///    {
		///        if (IsEnabled())
		///        {
		///            arg2 ??= "";
		///            fixed (char* string2Bytes = arg2)
		///            {
		///                EventSource.EventData* descrs = stackalloc EventSource.EventData[2];
		///                descrs[0].DataPointer = (IntPtr)(&amp;arg1);
		///                descrs[0].Size = 8;
		///                descrs[1].DataPointer = (IntPtr)string2Bytes;
		///                descrs[1].Size = ((arg2.Length + 1) * 2);
		///                WriteEventWithRelatedActivityIdCore(eventId, relatedActivityId, 2, descrs);
		///            }
		///        }
		///    }
		/// </code>
		/// </remarks>
		[UnconditionalSuppressMessage("ReflectionAnalysis", "IL2112:ReflectionToRequiresUnreferencedCode",
					Justification = "EnsureDescriptorsInitialized's use of GetType preserves this method which " +
									"requires unreferenced code, but EnsureDescriptorsInitialized does not access this member and is safe to call.")]
		[RequiresUnreferencedCode(EventSourceRequiresUnreferenceMessage)]
		[CLSCompliant(false)]
		protected unsafe void WriteEventWithRelatedActivityIdCore(int eventId, Guid* relatedActivityId, int eventDataCount, EventSource.EventData* data)
		{
			if (IsEnabled())
			{
				throw new PlatformNotSupportedException();
			}
		}

		// fallback varags helpers.
		/// <summary>
		/// This is the varargs helper for writing an event. It does create an array and box all the arguments so it is
		/// relatively inefficient and should only be used for relatively rare events (e.g. less than 100 / sec). If your
		/// rates are faster than that you should use <see cref="WriteEventCore"/> to create fast helpers for your particular
		/// method signature. Even if you use this for rare events, this call should be guarded by an <see cref="IsEnabled()"/>
		/// check so that the varargs call is not made when the EventSource is not active.
		/// </summary>
		[UnconditionalSuppressMessage("ReflectionAnalysis", "IL2112:ReflectionToRequiresUnreferencedCode",
					Justification = "EnsureDescriptorsInitialized's use of GetType preserves this method which " +
									"requires unreferenced code, but EnsureDescriptorsInitialized does not access this member and is safe to call.")]
		[RequiresUnreferencedCode(EventSourceRequiresUnreferenceMessage)]
		protected unsafe void WriteEvent(int eventId, params object?[] args)
		{
			throw new PlatformNotSupportedException();
		}

		/// <summary>
		/// This is the varargs helper for writing an event which also specifies a related activity. It is completely analogous
		/// to corresponding WriteEvent (they share implementation). It does create an array and box all the arguments so it is
		/// relatively inefficient and should only be used for relatively rare events (e.g. less than 100 / sec).  If your
		/// rates are faster than that you should use <see cref="WriteEventWithRelatedActivityIdCore"/> to create fast helpers for your
		/// particular method signature. Even if you use this for rare events, this call should be guarded by an <see cref="IsEnabled()"/>
		/// check so that the varargs call is not made when the EventSource is not active.
		/// </summary>
		[UnconditionalSuppressMessage("ReflectionAnalysis", "IL2112:ReflectionToRequiresUnreferencedCode",
					Justification = "EnsureDescriptorsInitialized's use of GetType preserves this method which " +
									"requires unreferenced code, but EnsureDescriptorsInitialized does not access this member and is safe to call.")]
		[RequiresUnreferencedCode(EventSourceRequiresUnreferenceMessage)]
		protected unsafe void WriteEventWithRelatedActivityId(int eventId, Guid relatedActivityId, params object?[] args)
		{
			throw new PlatformNotSupportedException();
		}

		#endregion protected

		#region IDisposable Members

		/// <summary>
		/// Disposes of an EventSource.
		/// </summary>
		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		/// <summary>
		/// Disposes of an EventSource.
		/// </summary>
		/// <remarks>
		/// Called from Dispose() with disposing=true, and from the finalizer (~EventSource) with disposing=false.
		/// Guidelines:
		/// 1. We may be called more than once: do nothing after the first call.
		/// 2. Avoid throwing exceptions if disposing is false, i.e. if we're being finalized.
		/// </remarks>
		/// <param name="disposing">True if called from Dispose(), false if called from the finalizer.</param>
		protected virtual void Dispose(bool disposing)
		{ }

		/// <summary>
		/// Finalizer for EventSource
		/// </summary>
		~EventSource()
		{
			this.Dispose(false);
		}

		#endregion IDisposable Members
	}

	/// <summary>
	/// Enables specifying event source configuration options to be used in the EventSource constructor.
	/// </summary>
	[Flags]
	public enum EventSourceSettings
	{
		/// <summary>
		/// This specifies none of the special configuration options should be enabled.
		/// </summary>
		Default = 0,

		/// <summary>
		/// Normally an EventSource NEVER throws; setting this option will tell it to throw when it encounters errors.
		/// </summary>
		ThrowOnEventWriteErrors = 1,

		/// <summary>
		/// Setting this option is a directive to the ETW listener should use manifest-based format when
		/// firing events. This is the default option when defining a type derived from EventSource
		/// (using the protected EventSource constructors).
		/// Only one of EtwManifestEventFormat or EtwSelfDescribingEventFormat should be specified
		/// </summary>
		EtwManifestEventFormat = 4,

		/// <summary>
		/// Setting this option is a directive to the ETW listener should use self-describing event format
		/// when firing events. This is the default option when creating a new instance of the EventSource
		/// type (using the public EventSource constructors).
		/// Only one of EtwManifestEventFormat or EtwSelfDescribingEventFormat should be specified
		/// </summary>
		EtwSelfDescribingEventFormat = 8,
	}

	/// <summary>
	/// An EventListener represents a target for the events generated by EventSources (that is subclasses
	/// of <see cref="EventSource"/>), in the current appdomain. When a new EventListener is created
	/// it is logically attached to all eventSources in that appdomain. When the EventListener is Disposed, then
	/// it is disconnected from the event eventSources. Note that there is a internal list of STRONG references
	/// to EventListeners, which means that relying on the lack of references to EventListeners to clean up
	/// EventListeners will NOT work. You must call EventListener.Dispose explicitly when a dispatcher is no
	/// longer needed.
	/// <para>
	/// Once created, EventListeners can enable or disable on a per-eventSource basis using verbosity levels
	/// (<see cref="EventLevel"/>) and bitfields (<see cref="EventKeywords"/>) to further restrict the set of
	/// events to be sent to the dispatcher. The dispatcher can also send arbitrary commands to a particular
	/// eventSource using the 'SendCommand' method. The meaning of the commands are eventSource specific.
	/// </para><para>
	/// The Null Guid (that is (new Guid()) has special meaning as a wildcard for 'all current eventSources in
	/// the appdomain'. Thus it is relatively easy to turn on all events in the appdomain if desired.
	/// </para><para>
	/// It is possible for there to be many EventListener's defined in a single appdomain. Each dispatcher is
	/// logically independent of the other listeners. Thus when one dispatcher enables or disables events, it
	/// affects only that dispatcher (other listeners get the events they asked for). It is possible that
	/// commands sent with 'SendCommand' would do a semantic operation that would affect the other listeners
	/// (like doing a GC, or flushing data ...), but this is the exception rather than the rule.
	/// </para><para>
	/// Thus the model is that each EventSource keeps a list of EventListeners that it is sending events
	/// to. Associated with each EventSource-dispatcher pair is a set of filtering criteria that determine for
	/// that eventSource what events that dispatcher will receive.
	/// </para><para>
	/// Listeners receive the events on their 'OnEventWritten' method. Thus subclasses of EventListener must
	/// override this method to do something useful with the data.
	/// </para><para>
	/// In addition, when new eventSources are created, the 'OnEventSourceCreate' method is called. The
	/// invariant associated with this callback is that every eventSource gets exactly one
	/// 'OnEventSourceCreate' call for ever eventSource that can potentially send it log messages. In
	/// particular when a EventListener is created, typically a series of OnEventSourceCreate' calls are
	/// made to notify the new dispatcher of all the eventSources that existed before the EventListener was
	/// created.
	/// </para>
	/// </summary>
	public class EventListener : IDisposable
	{
		private event EventHandler<EventSourceCreatedEventArgs>? _EventSourceCreated;

		/// <summary>
		/// This event is raised whenever a new eventSource is 'attached' to the dispatcher.
		/// This can happen for all existing EventSources when the EventListener is created
		/// as well as for any EventSources that come into existence after the EventListener
		/// has been created.
		///
		/// These 'catch up' events are called during the construction of the EventListener.
		/// Subclasses need to be prepared for that.
		///
		/// In a multi-threaded environment, it is possible that 'EventSourceEventWrittenCallback'
		/// events for a particular eventSource to occur BEFORE the EventSourceCreatedCallback is issued.
		/// </summary>
		public event EventHandler<EventSourceCreatedEventArgs>? EventSourceCreated
		{
			add
			{
				this._EventSourceCreated = (EventHandler<EventSourceCreatedEventArgs>?)Delegate.Combine(_EventSourceCreated, value);
			}
			remove
			{
				this._EventSourceCreated = (EventHandler<EventSourceCreatedEventArgs>?)Delegate.Remove(_EventSourceCreated, value);
			}
		}

		/// <summary>
		/// This event is raised whenever an event has been written by a EventSource for which
		/// the EventListener has enabled events.
		/// </summary>
		public event EventHandler<EventWrittenEventArgs>? EventWritten;

		static EventListener()
		{ }

		/// <summary>
		/// Create a new EventListener in which all events start off turned off (use EnableEvents to turn
		/// them on).
		/// </summary>
		public EventListener()
		{ }

		/// <summary>
		/// Dispose should be called when the EventListener no longer desires 'OnEvent*' callbacks. Because
		/// there is an internal list of strong references to all EventListeners, calling 'Dispose' directly
		/// is the only way to actually make the listen die. Thus it is important that users of EventListener
		/// call Dispose when they are done with their logging.
		/// </summary>
		public virtual void Dispose()
		{ }

		// We don't expose a Dispose(bool), because the contract is that you don't have any non-syncronous
		// 'cleanup' associated with this object

		/// <summary>
		/// Enable all events from the eventSource identified by 'eventSource' to the current
		/// dispatcher that have a verbosity level of 'level' or lower.
		///
		/// This call can have the effect of REDUCING the number of events sent to the
		/// dispatcher if 'level' indicates a less verbose level than was previously enabled.
		///
		/// This call never has an effect on other EventListeners.
		///
		/// </summary>
		public void EnableEvents(EventSource eventSource, EventLevel level)
		{
			EnableEvents(eventSource, level, EventKeywords.None);
		}

		/// <summary>
		/// Enable all events from the eventSource identified by 'eventSource' to the current
		/// dispatcher that have a verbosity level of 'level' or lower and have a event keyword
		/// matching any of the bits in 'matchAnyKeyword'.
		///
		/// This call can have the effect of REDUCING the number of events sent to the
		/// dispatcher if 'level' indicates a less verbose level than was previously enabled or
		/// if 'matchAnyKeyword' has fewer keywords set than where previously set.
		///
		/// This call never has an effect on other EventListeners.
		/// </summary>
		public void EnableEvents(EventSource eventSource, EventLevel level, EventKeywords matchAnyKeyword)
		{
			EnableEvents(eventSource, level, matchAnyKeyword, null);
		}

		/// <summary>
		/// Enable all events from the eventSource identified by 'eventSource' to the current
		/// dispatcher that have a verbosity level of 'level' or lower and have a event keyword
		/// matching any of the bits in 'matchAnyKeyword' as well as any (eventSource specific)
		/// effect passing additional 'key-value' arguments 'arguments' might have.
		///
		/// This call can have the effect of REDUCING the number of events sent to the
		/// dispatcher if 'level' indicates a less verbose level than was previously enabled or
		/// if 'matchAnyKeyword' has fewer keywords set than where previously set.
		///
		/// This call never has an effect on other EventListeners.
		/// </summary>
		public void EnableEvents(EventSource eventSource, EventLevel level, EventKeywords matchAnyKeyword, IDictionary<string, string?>? arguments)
		{
			if (eventSource == null)
			{
				throw new ArgumentNullException(nameof(eventSource));
			}
		}

		/// <summary>
		/// Disables all events coming from eventSource identified by 'eventSource'.
		///
		/// This call never has an effect on other EventListeners.
		/// </summary>
		public void DisableEvents(EventSource eventSource)
		{
			if (eventSource == null)
			{
				throw new ArgumentNullException(nameof(eventSource));
			}
		}

		/// <summary>
		/// EventSourceIndex is small non-negative integer (suitable for indexing in an array)
		/// identifying EventSource. It is unique per-appdomain. Some EventListeners might find
		/// it useful to store additional information about each eventSource connected to it,
		/// and EventSourceIndex allows this extra information to be efficiently stored in a
		/// (growable) array (eg List(T)).
		/// </summary>
		public static int EventSourceIndex(EventSource eventSource) { return 0; }

		/// <summary>
		/// This method is called whenever a new eventSource is 'attached' to the dispatcher.
		/// This can happen for all existing EventSources when the EventListener is created
		/// as well as for any EventSources that come into existence after the EventListener
		/// has been created.
		///
		/// These 'catch up' events are called during the construction of the EventListener.
		/// Subclasses need to be prepared for that.
		///
		/// In a multi-threaded environment, it is possible that 'OnEventWritten' callbacks
		/// for a particular eventSource to occur BEFORE the OnEventSourceCreated is issued.
		/// </summary>
		/// <param name="eventSource"></param>
		protected internal virtual void OnEventSourceCreated(EventSource eventSource)
		{
			EventHandler<EventSourceCreatedEventArgs>? callBack = this._EventSourceCreated;
			if (callBack != null)
			{
				EventSourceCreatedEventArgs args = new EventSourceCreatedEventArgs();
				args.EventSource = eventSource;
				callBack(this, args);
			}
		}

		/// <summary>
		/// This method is called whenever an event has been written by a EventSource for which
		/// the EventListener has enabled events.
		/// </summary>
		/// <param name="eventData"></param>
		protected internal virtual void OnEventWritten(EventWrittenEventArgs eventData)
		{
			this.EventWritten?.Invoke(this, eventData);
		}
	}

	/// <summary>
	/// Passed to the code:EventSource.OnEventCommand callback
	/// </summary>
	public class EventCommandEventArgs : EventArgs
	{
		/// <summary>
		/// Gets the command for the callback.
		/// </summary>
		public EventCommand Command { get; internal set; }

		/// <summary>
		/// Gets the arguments for the callback.
		/// </summary>
		public IDictionary<string, string?>? Arguments { get; internal set; }

		/// <summary>
		/// Enables the event that has the specified identifier.
		/// </summary>
		/// <param name="eventId">Event ID of event to be enabled</param>
		/// <returns>true if eventId is in range</returns>
		public bool EnableEvent(int eventId)
		{
			throw new PlatformNotSupportedException();
		}

		/// <summary>
		/// Disables the event that have the specified identifier.
		/// </summary>
		/// <param name="eventId">Event ID of event to be disabled</param>
		/// <returns>true if eventId is in range</returns>
		public bool DisableEvent(int eventId)
		{
			if (Command != EventCommand.Enable && Command != EventCommand.Disable)
				throw new InvalidOperationException();
			return eventSource.EnableEventForDispatcher(dispatcher, eventProviderType, eventId, false);
		}

		#region private

		internal EventCommandEventArgs(EventCommand command, IDictionary<string, string?>? arguments, EventSource eventSource,
			EventListener? listener, EventProviderType eventProviderType, int perEventSourceSessionId, int etwSessionId, bool enable, EventLevel level, EventKeywords matchAnyKeyword)
		{
			this.Command = command;
			this.Arguments = arguments;
			this.eventSource = eventSource;
			this.listener = listener;
			this.eventProviderType = eventProviderType;
			this.perEventSourceSessionId = perEventSourceSessionId;
			this.etwSessionId = etwSessionId;
			this.enable = enable;
			this.level = level;
			this.matchAnyKeyword = matchAnyKeyword;
		}

		internal EventSource eventSource;
		internal EventDispatcher? dispatcher;
		internal EventProviderType eventProviderType;

		// These are the arguments of sendCommand and are only used for deferring commands until after we are fully initialized.
		internal EventListener? listener;

		internal int perEventSourceSessionId;
		internal int etwSessionId;
		internal bool enable;
		internal EventLevel level;
		internal EventKeywords matchAnyKeyword;
		internal EventCommandEventArgs? nextCommand;     // We form a linked list of these deferred commands.

		#endregion private
	}

	/// <summary>
	/// EventSourceCreatedEventArgs is passed to <see cref="EventListener.EventSourceCreated"/>
	/// </summary>
	public class EventSourceCreatedEventArgs : EventArgs
	{
		/// <summary>
		/// The EventSource that is attaching to the listener.
		/// </summary>
		public EventSource? EventSource
		{
			get;
			internal set;
		}
	}

	/// <summary>
	/// EventWrittenEventArgs is passed to the user-provided override for
	/// <see cref="EventListener.OnEventWritten"/> when an event is fired.
	/// </summary>
	public class EventWrittenEventArgs : EventArgs
	{
		internal static readonly ReadOnlyCollection<object?> EmptyPayload = new(Array.Empty<object>());

		private ref EventSource.EventMetadata Metadata => ref EventSource.m_eventData![EventId];

		/// <summary>
		/// The name of the event.
		/// </summary>
		public string? EventName
		{
			get => _moreInfo?.EventName ?? (EventId <= 0 ? null : Metadata.Name);
			internal set => MoreInfo.EventName = value;
		}

		/// <summary>
		/// Gets the event ID for the event that was written.
		/// </summary>
		public int EventId { get; }

		private Guid _activityId;

		/// <summary>
		/// Gets the activity ID for the thread on which the event was written.
		/// </summary>
		public Guid ActivityId
		{
			get
			{
				if (_activityId == Guid.Empty)
				{
					_activityId = EventSource.CurrentThreadActivityId;
				}

				return _activityId;
			}
		}

		/// <summary>
		/// Gets the related activity ID if one was specified when the event was written.
		/// </summary>
		public Guid RelatedActivityId => _moreInfo?.RelatedActivityId ?? default;

		/// <summary>
		/// Gets the payload for the event.
		/// </summary>
		public ReadOnlyCollection<object?>? Payload { get; internal set; }

		/// <summary>
		/// Gets the payload argument names.
		/// </summary>
		public ReadOnlyCollection<string>? PayloadNames
		{
			get => _moreInfo?.PayloadNames ?? (EventId <= 0 ? null : Metadata.ParameterNames);
			internal set => MoreInfo.PayloadNames = value;
		}

		/// <summary>
		/// Gets the event source object.
		/// </summary>
		public EventSource EventSource { get; }

		/// <summary>
		/// Gets the keywords for the event.
		/// </summary>
		public EventKeywords Keywords
		{
			get => EventId <= 0 ? (_moreInfo?.Keywords ?? default) : (EventKeywords)Metadata.Descriptor.Keywords;
			internal set => MoreInfo.Keywords = value;
		}

		/// <summary>
		/// Gets the operation code for the event.
		/// </summary>
		public EventOpcode Opcode
		{
			get => EventId <= 0 ? (_moreInfo?.Opcode ?? default) : (EventOpcode)Metadata.Descriptor.Opcode;
			internal set => MoreInfo.Opcode = value;
		}

		/// <summary>
		/// Gets the task for the event.
		/// </summary>
		public EventTask Task => EventId <= 0 ? EventTask.None : (EventTask)Metadata.Descriptor.Task;

		/// <summary>
		/// Any provider/user defined options associated with the event.
		/// </summary>
		public EventTags Tags
		{
			get => EventId <= 0 ? (_moreInfo?.Tags ?? default) : Metadata.Tags;
			internal set => MoreInfo.Tags = value;
		}

		/// <summary>
		/// Gets the message for the event.  If the message has {N} parameters they are NOT substituted.
		/// </summary>
		public string? Message
		{
			get => _moreInfo?.Message ?? (EventId <= 0 ? null : Metadata.Message);
			internal set => MoreInfo.Message = value;
		}

#if FEATURE_MANAGED_ETW_CHANNELS

        /// <summary>
        /// Gets the channel for the event.
        /// </summary>
        public EventChannel Channel => EventId <= 0 ? EventChannel.None : (EventChannel)Metadata.Descriptor.Channel;
#endif

		/// <summary>
		/// Gets the version of the event.
		/// </summary>
		public byte Version => EventId <= 0 ? (byte)0 : Metadata.Descriptor.Version;

		/// <summary>
		/// Gets the level for the event.
		/// </summary>
		public EventLevel Level
		{
			get => EventId <= 0 ? (_moreInfo?.Level ?? default) : (EventLevel)Metadata.Descriptor.Level;
			internal set => MoreInfo.Level = value;
		}

		/// <summary>
		/// Gets the identifier for the OS thread that wrote the event.
		/// </summary>
		public long OSThreadId
		{
			get
			{
				ref long? osThreadId = ref MoreInfo.OsThreadId;
				if (!osThreadId.HasValue)
				{
#if ES_BUILD_STANDALONE
                    osThreadId = (long)Interop.Kernel32.GetCurrentThreadId();
#else
					osThreadId = (long)Thread.CurrentOSThreadId;
#endif
				}

				return osThreadId.Value;
			}
			internal set => MoreInfo.OsThreadId = value;
		}

		/// <summary>
		/// Gets a UTC DateTime that specifies when the event was written.
		/// </summary>
		public DateTime TimeStamp { get; internal set; }

		internal EventWrittenEventArgs(EventSource eventSource, int eventId)
		{
			EventSource = eventSource;
			EventId = eventId;
			TimeStamp = DateTime.UtcNow;
		}

		internal unsafe EventWrittenEventArgs(EventSource eventSource, int eventId, Guid* pActivityID, Guid* pChildActivityID)
			: this(eventSource, eventId)
		{
			if (pActivityID != null)
			{
				_activityId = *pActivityID;
			}

			if (pChildActivityID != null)
			{
				MoreInfo.RelatedActivityId = *pChildActivityID;
			}
		}

		private MoreEventInfo? _moreInfo;
		private MoreEventInfo MoreInfo => _moreInfo ??= new MoreEventInfo();

		private sealed class MoreEventInfo
		{
			public string? Message;
			public string? EventName;
			public ReadOnlyCollection<string>? PayloadNames;
			public Guid RelatedActivityId;
			public long? OsThreadId;
			public EventTags Tags;
			public EventOpcode Opcode;
			public EventLevel Level;
			public EventKeywords Keywords;
		}
	}

	/// <summary>
	/// Allows customizing defaults and specifying localization support for the event source class to which it is applied.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class)]
	public sealed class EventSourceAttribute : Attribute
	{
		/// <summary>
		/// Overrides the ETW name of the event source (which defaults to the class name)
		/// </summary>
		public string? Name { get; set; }

		/// <summary>
		/// Overrides the default (calculated) Guid of an EventSource type. Explicitly defining a GUID is discouraged,
		/// except when upgrading existing ETW providers to using event sources.
		/// </summary>
		public string? Guid { get; set; }

		/// <summary>
		/// <para>
		/// EventSources support localization of events. The names used for events, opcodes, tasks, keywords and maps
		/// can be localized to several languages if desired. This works by creating a ResX style string table
		/// (by simply adding a 'Resource File' to your project). This resource file is given a name e.g.
		/// 'DefaultNameSpace.ResourceFileName' which can be passed to the ResourceManager constructor to read the
		/// resources. This name is the value of the LocalizationResources property.
		/// </para><para>
		/// If LocalizationResources property is non-null, then EventSource will look up the localized strings for events by
		/// using the following resource naming scheme
		/// </para>
		///     <para>* event_EVENTNAME</para>
		///     <para>* task_TASKNAME</para>
		///     <para>* keyword_KEYWORDNAME</para>
		///     <para>* map_MAPNAME</para>
		/// <para>
		/// where the capitalized name is the name of the event, task, keyword, or map value that should be localized.
		/// Note that the localized string for an event corresponds to the Message string, and can have {0} values
		/// which represent the payload values.
		/// </para>
		/// </summary>
		public string? LocalizationResources { get; set; }
	}

	/// <summary>
	/// Any instance methods in a class that subclasses <see cref="EventSource"/> and that return void are
	/// assumed by default to be methods that generate an ETW event. Enough information can be deduced from the
	/// name of the method and its signature to generate basic schema information for the event. The
	/// <see cref="EventAttribute"/> class allows you to specify additional event schema information for an event if
	/// desired.
	/// </summary>
	[AttributeUsage(AttributeTargets.Method)]
	public sealed class EventAttribute : Attribute
	{
		/// <summary>Construct an EventAttribute with specified eventId</summary>
		/// <param name="eventId">ID of the ETW event (an integer between 1 and 65535)</param>
		public EventAttribute(int eventId)
		{
			this.EventId = eventId;
			Level = EventLevel.Informational;
		}

		/// <summary>Event's ID</summary>
		public int EventId { get; private set; }

		/// <summary>Event's severity level: indicates the severity or verbosity of the event</summary>
		public EventLevel Level { get; set; }

		/// <summary>Event's keywords: allows classification of events by "categories"</summary>
		public EventKeywords Keywords { get; set; }

		/// <summary>Event's operation code: allows defining operations, generally used with Tasks</summary>
		public EventOpcode Opcode
		{
			get => m_opcode;
			set
			{
				this.m_opcode = value;
				this.m_opcodeSet = true;
			}
		}

		internal bool IsOpcodeSet => m_opcodeSet;

		/// <summary>Event's task: allows logical grouping of events</summary>
		public EventTask Task { get; set; }

#if FEATURE_MANAGED_ETW_CHANNELS

        /// <summary>Event's channel: defines an event log as an additional destination for the event</summary>
        public EventChannel Channel { get; set; }
#endif

		/// <summary>Event's version</summary>
		public byte Version { get; set; }

		/// <summary>
		/// This can be specified to enable formatting and localization of the event's payload. You can
		/// use standard .NET substitution operators (eg {1}) in the string and they will be replaced
		/// with the 'ToString()' of the corresponding part of the  event payload.
		/// </summary>
		public string? Message { get; set; }

		/// <summary>
		/// User defined options associated with the event.  These do not have meaning to the EventSource but
		/// are passed through to listeners which given them semantics.
		/// </summary>
		public EventTags Tags { get; set; }

		/// <summary>
		/// Allows fine control over the Activity IDs generated by start and stop events
		/// </summary>
		public EventActivityOptions ActivityOptions { get; set; }

		#region private

		private EventOpcode m_opcode;
		private bool m_opcodeSet;

		#endregion private
	}

	/// <summary>
	/// By default all instance methods in a class that subclasses code:EventSource that and return
	/// void are assumed to be methods that generate an event. This default can be overridden by specifying
	/// the code:NonEventAttribute
	/// </summary>
	[AttributeUsage(AttributeTargets.Method)]
	public sealed class NonEventAttribute : Attribute
	{
		/// <summary>
		/// Constructs a default NonEventAttribute
		/// </summary>
		public NonEventAttribute() { }
	}

	// FUTURE we may want to expose this at some point once we have a partner that can help us validate the design.
#if FEATURE_MANAGED_ETW_CHANNELS

    /// <summary>
    /// EventChannelAttribute allows customizing channels supported by an EventSource. This attribute must be
    /// applied to an member of type EventChannel defined in a Channels class nested in the EventSource class:
    /// <code>
    ///     public static class Channels
    ///     {
    ///         [Channel(Enabled = true, EventChannelType = EventChannelType.Admin)]
    ///         public const EventChannel Admin = (EventChannel)16;
    ///
    ///         [Channel(Enabled = false, EventChannelType = EventChannelType.Operational)]
    ///         public const EventChannel Operational = (EventChannel)17;
    ///     }
    /// </code>
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
#if FEATURE_ADVANCED_MANAGED_ETW_CHANNELS
    public
#else
    internal
#endif
    class EventChannelAttribute : Attribute
    {
        /// <summary>
        /// Specified whether the channel is enabled by default
        /// </summary>
        public bool Enabled { get; set; }

        /// <summary>
        /// Legal values are in EventChannelType
        /// </summary>
        public EventChannelType EventChannelType { get; set; }

#if FEATURE_ADVANCED_MANAGED_ETW_CHANNELS

        /// <summary>
        /// Specifies the isolation for the channel
        /// </summary>
        public EventChannelIsolation Isolation { get; set; }

        /// <summary>
        /// Specifies an SDDL access descriptor that controls access to the log file that backs the channel.
        /// See MSDN (https://docs.microsoft.com/en-us/windows/desktop/WES/eventmanifestschema-channeltype-complextype) for details.
        /// </summary>
        public string? Access { get; set; }

        /// <summary>
        /// Allows importing channels defined in external manifests
        /// </summary>
        public string? ImportChannel { get; set; }
#endif

        // TODO: there is a convention that the name is the Provider/Type   Should we provide an override?
        // public string Name { get; set; }
    }

    /// <summary>
    /// Allowed channel types
    /// </summary>
#if FEATURE_ADVANCED_MANAGED_ETW_CHANNELS
    public
#else
    internal
#endif
    enum EventChannelType
    {
        /// <summary>The admin channel</summary>
        Admin = 1,

        /// <summary>The operational channel</summary>
        Operational,

        /// <summary>The Analytic channel</summary>
        Analytic,

        /// <summary>The debug channel</summary>
        Debug,
    }

#if FEATURE_ADVANCED_MANAGED_ETW_CHANNELS

    /// <summary>
    /// Allowed isolation levels. See MSDN (https://docs.microsoft.com/en-us/windows/desktop/WES/eventmanifestschema-channeltype-complextype)
    /// for the default permissions associated with each level. EventChannelIsolation and Access allows control over the
    /// access permissions for the channel and backing file.
    /// </summary>
    public
    enum EventChannelIsolation
    {
        /// <summary>
        /// This is the default isolation level. All channels that specify Application isolation use the same ETW session
        /// </summary>
        Application = 1,

        /// <summary>
        /// All channels that specify System isolation use the same ETW session
        /// </summary>
        System,

        /// <summary>
        /// Use sparingly! When specifying Custom isolation, a separate ETW session is created for the channel.
        /// Using Custom isolation lets you control the access permissions for the channel and backing file.
        /// Because there are only 64 ETW sessions available, you should limit your use of Custom isolation.
        /// </summary>
        Custom,
    }
#endif
#endif

	/// <summary>
	/// Describes the pre-defined command (EventCommandEventArgs.Command property) that is passed to the OnEventCommand callback.
	/// </summary>
	public enum EventCommand
	{
		/// <summary>
		/// Update EventSource state
		/// </summary>
		Update = 0,

		/// <summary>
		/// Request EventSource to generate and send its manifest
		/// </summary>
		SendManifest = -1,

		/// <summary>
		/// Enable event
		/// </summary>
		Enable = -2,

		/// <summary>
		/// Disable event
		/// </summary>
		Disable = -3
	}

	#region private classes

	// holds a bitfield representing a session mask
	/// <summary>
	/// A SessionMask represents a set of (at most MAX) sessions as a bit mask. The perEventSourceSessionId
	/// is the index in the SessionMask of the bit that will be set. These can translate to
	/// EventSource's reserved keywords bits using the provided ToEventKeywords() and
	/// FromEventKeywords() methods.
	/// </summary>
	internal struct SessionMask
	{
		public SessionMask(SessionMask m)
		{ m_mask = m.m_mask; }

		public SessionMask(uint mask = 0)
		{ m_mask = mask & MASK; }

		public bool IsEqualOrSupersetOf(SessionMask m)
		{
			return (this.m_mask | m.m_mask) == this.m_mask;
		}

		public static SessionMask All => new SessionMask(MASK);

		public static SessionMask FromId(int perEventSourceSessionId)
		{
			Debug.Assert(perEventSourceSessionId < MAX);
			return new SessionMask((uint)1 << perEventSourceSessionId);
		}

		public ulong ToEventKeywords()
		{
			return (ulong)m_mask << SHIFT_SESSION_TO_KEYWORD;
		}

		public static SessionMask FromEventKeywords(ulong m)
		{
			return new SessionMask((uint)(m >> SHIFT_SESSION_TO_KEYWORD));
		}

		public bool this[int perEventSourceSessionId]
		{
			get
			{
				Debug.Assert(perEventSourceSessionId < MAX);
				return (m_mask & (1 << perEventSourceSessionId)) != 0;
			}
			set
			{
				Debug.Assert(perEventSourceSessionId < MAX);
				if (value) m_mask |= ((uint)1 << perEventSourceSessionId);
				else m_mask &= ~((uint)1 << perEventSourceSessionId);
			}
		}

		public static SessionMask operator |(SessionMask m1, SessionMask m2) =>
			new SessionMask(m1.m_mask | m2.m_mask);

		public static SessionMask operator &(SessionMask m1, SessionMask m2) =>
			new SessionMask(m1.m_mask & m2.m_mask);

		public static SessionMask operator ^(SessionMask m1, SessionMask m2) =>
			new SessionMask(m1.m_mask ^ m2.m_mask);

		public static SessionMask operator ~(SessionMask m) =>
			new SessionMask(MASK & ~(m.m_mask));

		public static explicit operator ulong(SessionMask m) => m.m_mask;

		public static explicit operator uint(SessionMask m) => m.m_mask;

		private uint m_mask;

		internal const int SHIFT_SESSION_TO_KEYWORD = 44;         // bits 44-47 inclusive are reserved
		internal const uint MASK = 0x0fU;                         // the mask of 4 reserved bits
		internal const uint MAX = 4;                              // maximum number of simultaneous ETW sessions supported
	}

	/// <summary>
	/// code:EventDispatchers are a simple 'helper' structure that holds the filtering state
	/// (m_EventEnabled) for a particular EventSource X EventListener tuple
	///
	/// Thus a single EventListener may have many EventDispatchers (one for every EventSource
	/// that EventListener has activate) and a Single EventSource may also have many
	/// event Dispatchers (one for every EventListener that has activated it).
	///
	/// Logically a particular EventDispatcher belongs to exactly one EventSource and exactly
	/// one EventListener (although EventDispatcher does not 'remember' the EventSource it is
	/// associated with.
	/// </summary>
	internal sealed class EventDispatcher
	{
		internal EventDispatcher(EventDispatcher? next, bool[]? eventEnabled, EventListener listener)
		{
			m_Next = next;
			m_EventEnabled = eventEnabled;
			m_Listener = listener;
		}

		// Instance fields
		internal readonly EventListener m_Listener;   // The dispatcher this entry is for

		internal bool[]? m_EventEnabled;              // For every event in a the eventSource, is it enabled?

		// Only guaranteed to exist after a InsureInit()
		internal EventDispatcher? m_Next;              // These form a linked list in code:EventSource.m_Dispatchers

		// Of all listeners for that eventSource.
	}

	/// <summary>
	/// Flags that can be used with EventSource.GenerateManifest to control how the ETW manifest for the EventSource is
	/// generated.
	/// </summary>
	[Flags]
	public enum EventManifestOptions
	{
		/// <summary>
		/// Only the resources associated with current UI culture are included in the  manifest
		/// </summary>
		None = 0x0,

		/// <summary>
		/// Throw exceptions for any inconsistency encountered
		/// </summary>
		Strict = 0x1,

		/// <summary>
		/// Generate a "resources" node under "localization" for every satellite assembly provided
		/// </summary>
		AllCultures = 0x2,

		/// <summary>
		/// Generate the manifest only if the event source needs to be registered on the machine,
		/// otherwise return null (but still perform validation if Strict is specified)
		/// </summary>
		OnlyIfNeededForRegistration = 0x4,

		/// <summary>
		/// When generating the manifest do *not* enforce the rule that the current EventSource class
		/// must be the base class for the user-defined type passed in. This allows validation of .net
		/// event sources using the new validation code
		/// </summary>
		AllowEventSourceOverride = 0x8,
	}

	/// <summary>
	/// ManifestBuilder is designed to isolate the details of the message of the event from the
	/// rest of EventSource.  This one happens to create XML.
	/// </summary>
	internal sealed class ManifestBuilder
	{
		/// <summary>
		/// Build a manifest for 'providerName' with the given GUID, which will be packaged into 'dllName'.
		/// 'resources, is a resource manager.  If specified all messages are localized using that manager.
		/// </summary>
		public ManifestBuilder(string providerName, Guid providerGuid, string? dllName, ResourceManager? resources,
							   EventManifestOptions flags)
		{
#if FEATURE_MANAGED_ETW_CHANNELS
            this.providerName = providerName;
#endif
			this.flags = flags;

			this.resources = resources;
			sb = new StringBuilder();
			events = new StringBuilder();
			templates = new StringBuilder();
			opcodeTab = new Dictionary<int, string>();
			stringTab = new Dictionary<string, string>();
			errors = new List<string>();
			perEventByteArrayArgIndices = new Dictionary<string, List<int>>();

			sb.AppendLine("<instrumentationManifest xmlns=\"http://schemas.microsoft.com/win/2004/08/events\">");
			sb.AppendLine(" <instrumentation xmlns:xs=\"http://www.w3.org/2001/XMLSchema\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:win=\"http://manifests.microsoft.com/win/2004/08/windows/events\">");
			sb.AppendLine("  <events xmlns=\"http://schemas.microsoft.com/win/2004/08/events\">");
			sb.Append($"<provider name=\"{providerName}\" guid=\"{{{providerGuid}}}\"");
			if (dllName != null)
				sb.Append($" resourceFileName=\"{dllName}\" messageFileName=\"{dllName}\"");

			string symbolsName = providerName.Replace("-", "").Replace('.', '_');  // Period and - are illegal replace them.
			sb.AppendLine($" symbol=\"{symbolsName}\">");
		}

		public void AddOpcode(string name, int value)
		{
			if ((flags & EventManifestOptions.Strict) != 0)
			{
				if (value <= 10 || value >= 239)
				{
					ManifestError(SR.Format(SR.EventSource_IllegalOpcodeValue, name, value));
				}

				if (opcodeTab.TryGetValue(value, out string? prevName) && !name.Equals(prevName, StringComparison.Ordinal))
				{
					ManifestError(SR.Format(SR.EventSource_OpcodeCollision, name, prevName, value));
				}
			}

			opcodeTab[value] = name;
		}

		public void AddTask(string name, int value)
		{
			if ((flags & EventManifestOptions.Strict) != 0)
			{
				if (value <= 0 || value >= 65535)
				{
					ManifestError(SR.Format(SR.EventSource_IllegalTaskValue, name, value));
				}

				if (taskTab != null && taskTab.TryGetValue(value, out string? prevName) && !name.Equals(prevName, StringComparison.Ordinal))
				{
					ManifestError(SR.Format(SR.EventSource_TaskCollision, name, prevName, value));
				}
			}

			taskTab ??= new Dictionary<int, string>();
			taskTab[value] = name;
		}

		public void AddKeyword(string name, ulong value)
		{
			if ((value & (value - 1)) != 0)   // Is it a power of 2?
			{
				ManifestError(SR.Format(SR.EventSource_KeywordNeedPowerOfTwo, "0x" + value.ToString("x", CultureInfo.CurrentCulture), name), true);
			}
			if ((flags & EventManifestOptions.Strict) != 0)
			{
				if (value >= 0x0000100000000000UL && !name.StartsWith("Session", StringComparison.Ordinal))
				{
					ManifestError(SR.Format(SR.EventSource_IllegalKeywordsValue, name, "0x" + value.ToString("x", CultureInfo.CurrentCulture)));
				}

				if (keywordTab != null && keywordTab.TryGetValue(value, out string? prevName) && !name.Equals(prevName, StringComparison.Ordinal))
				{
					ManifestError(SR.Format(SR.EventSource_KeywordCollision, name, prevName, "0x" + value.ToString("x", CultureInfo.CurrentCulture)));
				}
			}

			keywordTab ??= new Dictionary<ulong, string>();
			keywordTab[value] = name;
		}

#if FEATURE_MANAGED_ETW_CHANNELS

        /// <summary>
        /// Add a channel.  channelAttribute can be null
        /// </summary>
        public void AddChannel(string? name, int value, EventChannelAttribute? channelAttribute)
        {
            EventChannel chValue = (EventChannel)value;
            if (value < (int)EventChannel.Admin || value > 255)
                ManifestError(SR.Format(SR.EventSource_EventChannelOutOfRange, name, value));
            else if (chValue >= EventChannel.Admin && chValue <= EventChannel.Debug &&
                     channelAttribute != null && EventChannelToChannelType(chValue) != channelAttribute.EventChannelType)
            {
                // we want to ensure developers do not define EventChannels that conflict with the builtin ones,
                // but we want to allow them to override the default ones...
                ManifestError(SR.Format(SR.EventSource_ChannelTypeDoesNotMatchEventChannelValue,
                                                                            name, ((EventChannel)value).ToString()));
            }

            // TODO: validate there are no conflicting manifest exposed names (generally following the format "provider/type")

            ulong kwd = GetChannelKeyword(chValue);

            channelTab ??= new Dictionary<int, ChannelInfo>(4);
            channelTab[value] = new ChannelInfo { Name = name, Keywords = kwd, Attribs = channelAttribute };
        }

        private static EventChannelType EventChannelToChannelType(EventChannel channel)
        {
#if !ES_BUILD_STANDALONE
            Debug.Assert(channel >= EventChannel.Admin && channel <= EventChannel.Debug);
#endif
            return (EventChannelType)((int)channel - (int)EventChannel.Admin + (int)EventChannelType.Admin);
        }

        private static EventChannelAttribute GetDefaultChannelAttribute(EventChannel channel)
        {
            EventChannelAttribute attrib = new EventChannelAttribute();
            attrib.EventChannelType = EventChannelToChannelType(channel);
            if (attrib.EventChannelType <= EventChannelType.Operational)
                attrib.Enabled = true;
            return attrib;
        }

        public ulong[] GetChannelData()
        {
            if (this.channelTab == null)
            {
                return Array.Empty<ulong>();
            }

            // We create an array indexed by the channel id for fast look up.
            // E.g. channelMask[Admin] will give you the bit mask for Admin channel.
            int maxkey = -1;
            foreach (int item in this.channelTab.Keys)
            {
                if (item > maxkey)
                {
                    maxkey = item;
                }
            }

            ulong[] channelMask = new ulong[maxkey + 1];
            foreach (KeyValuePair<int, ChannelInfo> item in this.channelTab)
            {
                channelMask[item.Key] = item.Value.Keywords;
            }

            return channelMask;
        }

#endif

		public void StartEvent(string eventName, EventAttribute eventAttribute)
		{
			Debug.Assert(numParams == 0);
			Debug.Assert(this.eventName == null);
			this.eventName = eventName;
			numParams = 0;
			byteArrArgIndices = null;

			events.Append("  <event value=\"").Append(eventAttribute.EventId).
				 Append("\" version=\"").Append(eventAttribute.Version).
				 Append("\" level=\"");
			AppendLevelName(events, eventAttribute.Level);
			events.Append("\" symbol=\"").Append(eventName).Append('"');

			// at this point we add to the manifest's stringTab a message that is as-of-yet
			// "untranslated to manifest convention", b/c we don't have the number or position
			// of any byte[] args (which require string format index updates)
			WriteMessageAttrib(events, "event", eventName, eventAttribute.Message);

			if (eventAttribute.Keywords != 0)
			{
				events.Append(" keywords=\"");
				AppendKeywords(events, (ulong)eventAttribute.Keywords, eventName);
				events.Append('"');
			}

			if (eventAttribute.Opcode != 0)
			{
				events.Append(" opcode=\"").Append(GetOpcodeName(eventAttribute.Opcode, eventName)).Append('"');
			}

			if (eventAttribute.Task != 0)
			{
				events.Append(" task=\"").Append(GetTaskName(eventAttribute.Task, eventName)).Append('"');
			}

#if FEATURE_MANAGED_ETW_CHANNELS
            if (eventAttribute.Channel != 0)
            {
                events.Append(" channel=\"").Append(GetChannelName(eventAttribute.Channel, eventName, eventAttribute.Message)).Append('"');
            }
#endif
		}

		public void AddEventParameter(Type type, string name)
		{
			if (numParams == 0)
				templates.Append("  <template tid=\"").Append(eventName).AppendLine("Args\">");
			if (type == typeof(byte[]))
			{
				// mark this index as "extraneous" (it has no parallel in the managed signature)
				// we use these values in TranslateToManifestConvention()
				byteArrArgIndices ??= new List<int>(4);
				byteArrArgIndices.Add(numParams);

				// add an extra field to the template representing the length of the binary blob
				numParams++;
				templates.Append("   <data name=\"").Append(name).AppendLine("Size\" inType=\"win:UInt32\"/>");
			}
			numParams++;
			templates.Append("   <data name=\"").Append(name).Append("\" inType=\"").Append(GetTypeName(type)).Append('"');

			// TODO: for 'byte*' types it assumes the user provided length is named using the same naming convention
			//       as for 'byte[]' args (blob_arg_name + "Size")
			if ((type.IsArray || type.IsPointer) && type.GetElementType() == typeof(byte))
			{
				// add "length" attribute to the "blob" field in the template (referencing the field added above)
				templates.Append(" length=\"").Append(name).Append("Size\"");
			}

			// ETW does not support 64-bit value maps, so we don't specify these as ETW maps
			if (type.IsEnum && Enum.GetUnderlyingType(type) != typeof(ulong) && Enum.GetUnderlyingType(type) != typeof(long))
			{
				templates.Append(" map=\"").Append(type.Name).Append('"');
				mapsTab ??= new Dictionary<string, Type>();
				if (!mapsTab.ContainsKey(type.Name))
					mapsTab.Add(type.Name, type);        // Remember that we need to dump the type enumeration
			}

			templates.AppendLine("/>");
		}

		public void EndEvent()
		{
			Debug.Assert(eventName != null);

			if (numParams > 0)
			{
				templates.AppendLine("  </template>");
				events.Append(" template=\"").Append(eventName).Append("Args\"");
			}
			events.AppendLine("/>");

			if (byteArrArgIndices != null)
				perEventByteArrayArgIndices[eventName] = byteArrArgIndices;

			// at this point we have all the information we need to translate the C# Message
			// to the manifest string we'll put in the stringTab
			string prefixedEventName = "event_" + eventName;
			if (stringTab.TryGetValue(prefixedEventName, out string? msg))
			{
				msg = TranslateToManifestConvention(msg, eventName);
				stringTab[prefixedEventName] = msg;
			}

			eventName = null;
			numParams = 0;
			byteArrArgIndices = null;
		}

#if FEATURE_MANAGED_ETW_CHANNELS

        // Channel keywords are generated one per channel to allow channel based filtering in event viewer. These keywords are autogenerated
        // by mc.exe for compiling a manifest and are based on the order of the channels (fields) in the Channels inner class (when advanced
        // channel support is enabled), or based on the order the predefined channels appear in the EventAttribute properties (for simple
        // support). The manifest generated *MUST* have the channels specified in the same order (that's how our computed keywords are mapped
        // to channels by the OS infrastructure).
        // If channelKeyworkds is present, and has keywords bits in the ValidPredefinedChannelKeywords then it is
        // assumed that the keyword for that channel should be that bit.
        // otherwise we allocate a channel bit for the channel.
        // explicit channel bits are only used by WCF to mimic an existing manifest,
        // so we don't dont do error checking.
        public ulong GetChannelKeyword(EventChannel channel, ulong channelKeyword = 0)
        {
            // strip off any non-channel keywords, since we are only interested in channels here.
            channelKeyword &= ValidPredefinedChannelKeywords;
            channelTab ??= new Dictionary<int, ChannelInfo>(4);

            if (channelTab.Count == MaxCountChannels)
                ManifestError(SR.EventSource_MaxChannelExceeded);

            if (!channelTab.TryGetValue((int)channel, out ChannelInfo? info))
            {
                // If we were not given an explicit channel, allocate one.
                if (channelKeyword == 0)
                {
                    channelKeyword = nextChannelKeywordBit;
                    nextChannelKeywordBit >>= 1;
                }
            }
            else
            {
                channelKeyword = info.Keywords;
            }

            return channelKeyword;
        }
#endif

		public byte[] CreateManifest()
		{
			string str = CreateManifestString();
			return Encoding.UTF8.GetBytes(str);
		}

		public IList<string> Errors => errors;

		public bool HasResources => resources != null;

		/// <summary>
		/// When validating an event source it adds the error to the error collection.
		/// When not validating it throws an exception if runtimeCritical is "true".
		/// Otherwise the error is ignored.
		/// </summary>
		/// <param name="msg"></param>
		/// <param name="runtimeCritical"></param>
		public void ManifestError(string msg, bool runtimeCritical = false)
		{
			if ((flags & EventManifestOptions.Strict) != 0)
				errors.Add(msg);
			else if (runtimeCritical)
				throw new ArgumentException(msg);
		}

		private string CreateManifestString()
		{
#if !ES_BUILD_STANDALONE
			Span<char> ulongHexScratch = stackalloc char[16]; // long enough for ulong.MaxValue formatted as hex
#endif

#if FEATURE_MANAGED_ETW_CHANNELS

            // Write out the channels
            if (channelTab != null)
            {
                sb.AppendLine(" <channels>");
                var sortedChannels = new List<KeyValuePair<int, ChannelInfo>>();
                foreach (KeyValuePair<int, ChannelInfo> p in channelTab) { sortedChannels.Add(p); }
                sortedChannels.Sort((p1, p2) => -Comparer<ulong>.Default.Compare(p1.Value.Keywords, p2.Value.Keywords));
                foreach (KeyValuePair<int, ChannelInfo> kvpair in sortedChannels)
                {
                    int channel = kvpair.Key;
                    ChannelInfo channelInfo = kvpair.Value;

                    string? channelType = null;
                    bool enabled = false;
                    string? fullName = null;
#if FEATURE_ADVANCED_MANAGED_ETW_CHANNELS
                    string? isolation = null;
                    string? access = null;
#endif
                    if (channelInfo.Attribs != null)
                    {
                        EventChannelAttribute attribs = channelInfo.Attribs;
                        if (Enum.IsDefined(typeof(EventChannelType), attribs.EventChannelType))
                            channelType = attribs.EventChannelType.ToString();
                        enabled = attribs.Enabled;
#if FEATURE_ADVANCED_MANAGED_ETW_CHANNELS
                        if (attribs.ImportChannel != null)
                        {
                            fullName = attribs.ImportChannel;
                            elementName = "importChannel";
                        }
                        if (Enum.IsDefined(typeof(EventChannelIsolation), attribs.Isolation))
                            isolation = attribs.Isolation.ToString();
                        access = attribs.Access;
#endif
                    }

                    fullName ??= providerName + "/" + channelInfo.Name;

                    sb.Append("  <channel chid=\"").Append(channelInfo.Name).Append("\" name=\"").Append(fullName).Append('"');

                    Debug.Assert(channelInfo.Name != null);
                    WriteMessageAttrib(sb, "channel", channelInfo.Name, null);
                    sb.Append(" value=\"").Append(channel).Append('"');
                    if (channelType != null)
                        sb.Append(" type=\"").Append(channelType).Append('"');
                    sb.Append(" enabled=\"").Append(enabled ? "true" : "false").Append('"');
#if FEATURE_ADVANCED_MANAGED_ETW_CHANNELS
                    if (access != null)
                        sb.Append(" access=\"").Append(access).Append("\"");
                    if (isolation != null)
                        sb.Append(" isolation=\"").Append(isolation).Append("\"");
#endif
                    sb.AppendLine("/>");
                }
                sb.AppendLine(" </channels>");
            }
#endif

			// Write out the tasks
			if (taskTab != null)
			{
				sb.AppendLine(" <tasks>");
				var sortedTasks = new List<int>(taskTab.Keys);
				sortedTasks.Sort();
				foreach (int task in sortedTasks)
				{
					sb.Append("  <task");
					WriteNameAndMessageAttribs(sb, "task", taskTab[task]);
					sb.Append(" value=\"").Append(task).AppendLine("\"/>");
				}
				sb.AppendLine(" </tasks>");
			}

			// Write out the maps

			// Scoping the call to enum GetFields to a local function to limit the linker suppression
#if !ES_BUILD_STANDALONE
			[UnconditionalSuppressMessage("ReflectionAnalysis", "IL2070:UnrecognizedReflectionPattern",
			Justification = "Trimmer does not trim enums")]
#endif
			static FieldInfo[] GetEnumFields(Type localEnumType)
			{
				Debug.Assert(localEnumType.IsEnum);
				return localEnumType.GetFields(BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Static);
			}

			if (mapsTab != null)
			{
				sb.AppendLine(" <maps>");
				foreach (Type enumType in mapsTab.Values)
				{
					bool isbitmap = EventSource.IsCustomAttributeDefinedHelper(enumType, typeof(FlagsAttribute), flags);
					string mapKind = isbitmap ? "bitMap" : "valueMap";
					sb.Append("  <").Append(mapKind).Append(" name=\"").Append(enumType.Name).AppendLine("\">");

					// write out each enum value
					FieldInfo[] staticFields = GetEnumFields(enumType);
					bool anyValuesWritten = false;
					foreach (FieldInfo staticField in staticFields)
					{
						object? constantValObj = staticField.GetRawConstantValue();

						if (constantValObj != null)
						{
							ulong hexValue;
							if (constantValObj is ulong)
								hexValue = (ulong)constantValObj;    // This is the only integer type that can't be represented by a long.
							else
								hexValue = (ulong)Convert.ToInt64(constantValObj); // Handles all integer types except ulong.

							// ETW requires all bitmap values to be powers of 2.  Skip the ones that are not.
							// TODO: Warn people about the dropping of values.
							if (isbitmap && ((hexValue & (hexValue - 1)) != 0 || hexValue == 0))
								continue;

#if ES_BUILD_STANDALONE
                            string hexValueFormatted = hexValue.ToString("x", CultureInfo.InvariantCulture);
#else
							hexValue.TryFormat(ulongHexScratch, out int charsWritten, "x");
							Span<char> hexValueFormatted = ulongHexScratch.Slice(0, charsWritten);
#endif
							sb.Append("   <map value=\"0x").Append(hexValueFormatted).Append('"');
							WriteMessageAttrib(sb, "map", enumType.Name + "." + staticField.Name, staticField.Name);
							sb.AppendLine("/>");
							anyValuesWritten = true;
						}
					}

					// the OS requires that bitmaps and valuemaps have at least one value or it reject the whole manifest.
					// To avoid that put a 'None' entry if there are no other values.
					if (!anyValuesWritten)
					{
						sb.Append("   <map value=\"0x0\"");
						WriteMessageAttrib(sb, "map", enumType.Name + ".None", "None");
						sb.AppendLine("/>");
					}
					sb.Append("  </").Append(mapKind).AppendLine(">");
				}
				sb.AppendLine(" </maps>");
			}

			// Write out the opcodes
			sb.AppendLine(" <opcodes>");
			var sortedOpcodes = new List<int>(opcodeTab.Keys);
			sortedOpcodes.Sort();
			foreach (int opcode in sortedOpcodes)
			{
				sb.Append("  <opcode");
				WriteNameAndMessageAttribs(sb, "opcode", opcodeTab[opcode]);
				sb.Append(" value=\"").Append(opcode).AppendLine("\"/>");
			}
			sb.AppendLine(" </opcodes>");

			// Write out the keywords
			if (keywordTab != null)
			{
				sb.AppendLine(" <keywords>");
				var sortedKeywords = new List<ulong>(keywordTab.Keys);
				sortedKeywords.Sort();
				foreach (ulong keyword in sortedKeywords)
				{
					sb.Append("  <keyword");
					WriteNameAndMessageAttribs(sb, "keyword", keywordTab[keyword]);
#if ES_BUILD_STANDALONE
                    string keywordFormatted = keyword.ToString("x", CultureInfo.InvariantCulture);
#else
					keyword.TryFormat(ulongHexScratch, out int charsWritten, "x");
					Span<char> keywordFormatted = ulongHexScratch.Slice(0, charsWritten);
#endif
					sb.Append(" mask=\"0x").Append(keywordFormatted).AppendLine("\"/>");
				}
				sb.AppendLine(" </keywords>");
			}

			sb.AppendLine(" <events>");
			sb.Append(events);
			sb.AppendLine(" </events>");

			sb.AppendLine(" <templates>");
			if (templates.Length > 0)
			{
				sb.Append(templates);
			}
			else
			{
				// Work around a cornercase ETW issue where a manifest with no templates causes
				// ETW events to not get sent to their associated channel.
				sb.AppendLine("    <template tid=\"_empty\"></template>");
			}
			sb.AppendLine(" </templates>");

			sb.AppendLine("</provider>");
			sb.AppendLine("</events>");
			sb.AppendLine("</instrumentation>");

			// Output the localization information.
			sb.AppendLine("<localization>");

			var sortedStrings = new string[stringTab.Keys.Count];
			stringTab.Keys.CopyTo(sortedStrings, 0);
			Array.Sort<string>(sortedStrings, 0, sortedStrings.Length);

			CultureInfo ci = CultureInfo.CurrentUICulture;
			sb.Append(" <resources culture=\"").Append(ci.Name).AppendLine("\">");
			sb.AppendLine("  <stringTable>");
			foreach (string stringKey in sortedStrings)
			{
				string? val = GetLocalizedMessage(stringKey, ci, etwFormat: true);
				sb.Append("   <string id=\"").Append(stringKey).Append("\" value=\"").Append(val).AppendLine("\"/>");
			}
			sb.AppendLine("  </stringTable>");
			sb.AppendLine(" </resources>");

			sb.AppendLine("</localization>");
			sb.AppendLine("</instrumentationManifest>");
			return sb.ToString();
		}

		#region private

		private void WriteNameAndMessageAttribs(StringBuilder stringBuilder, string elementName, string name)
		{
			stringBuilder.Append(" name=\"").Append(name).Append('"');
			WriteMessageAttrib(sb, elementName, name, name);
		}

		private void WriteMessageAttrib(StringBuilder stringBuilder, string elementName, string name, string? value)
		{
			string? key = null;

			// See if the user wants things localized.
			if (resources != null)
			{
				// resource fallback: strings in the neutral culture will take precedence over inline strings
				key = elementName + "_" + name;
				if (resources.GetString(key, CultureInfo.InvariantCulture) is string localizedString)
					value = localizedString;
			}

			if (value == null)
				return;

			key ??= elementName + "_" + name;
			stringBuilder.Append(" message=\"$(string.").Append(key).Append(")\"");

			if (stringTab.TryGetValue(key, out string? prevValue) && !prevValue.Equals(value))
			{
				ManifestError(SR.Format(SR.EventSource_DuplicateStringKey, key), true);
				return;
			}

			stringTab[key] = value;
		}

		internal string? GetLocalizedMessage(string key, CultureInfo ci, bool etwFormat)
		{
			string? value = null;
			if (resources != null)
			{
				string? localizedString = resources.GetString(key, ci);
				if (localizedString != null)
				{
					value = localizedString;
					if (etwFormat && key.StartsWith("event_", StringComparison.Ordinal))
					{
						string evtName = key.Substring("event_".Length);
						value = TranslateToManifestConvention(value, evtName);
					}
				}
			}
			if (etwFormat && value == null)
				stringTab.TryGetValue(key, out value);

			return value;
		}

		private static void AppendLevelName(StringBuilder sb, EventLevel level)
		{
			if ((int)level < 16)
			{
				sb.Append("win:");
			}

			sb.Append(level switch // avoid boxing that comes from level.ToString()
			{
				EventLevel.LogAlways => nameof(EventLevel.LogAlways),
				EventLevel.Critical => nameof(EventLevel.Critical),
				EventLevel.Error => nameof(EventLevel.Error),
				EventLevel.Warning => nameof(EventLevel.Warning),
				EventLevel.Informational => nameof(EventLevel.Informational),
				EventLevel.Verbose => nameof(EventLevel.Verbose),
				_ => ((int)level).ToString()
			});
		}

#if FEATURE_MANAGED_ETW_CHANNELS
        private string? GetChannelName(EventChannel channel, string eventName, string? eventMessage)
        {
            if (channelTab == null || !channelTab.TryGetValue((int)channel, out ChannelInfo? info))
            {
                if (channel < EventChannel.Admin) // || channel > EventChannel.Debug)
                    ManifestError(SR.Format(SR.EventSource_UndefinedChannel, channel, eventName));

                // allow channels to be auto-defined.  The well known ones get their well known names, and the
                // rest get names Channel<N>.  This allows users to modify the Manifest if they want more advanced features.
                channelTab ??= new Dictionary<int, ChannelInfo>(4);

                string channelName = channel.ToString();        // For well know channels this is a nice name, otherwise a number
                if (EventChannel.Debug < channel)
                    channelName = "Channel" + channelName;      // Add a 'Channel' prefix for numbers.

                AddChannel(channelName, (int)channel, GetDefaultChannelAttribute(channel));
                if (!channelTab.TryGetValue((int)channel, out info))
                    ManifestError(SR.Format(SR.EventSource_UndefinedChannel, channel, eventName));
            }

            // events that specify admin channels *must* have non-null "Message" attributes
            if (resources != null)
                eventMessage ??= resources.GetString("event_" + eventName, CultureInfo.InvariantCulture);

            Debug.Assert(info!.Attribs != null);
            if (info.Attribs.EventChannelType == EventChannelType.Admin && eventMessage == null)
                ManifestError(SR.Format(SR.EventSource_EventWithAdminChannelMustHaveMessage, eventName, info.Name));
            return info.Name;
        }
#endif

		private string GetTaskName(EventTask task, string eventName)
		{
			if (task == EventTask.None)
				return "";

			taskTab ??= new Dictionary<int, string>();
			if (!taskTab.TryGetValue((int)task, out string? ret))
				ret = taskTab[(int)task] = eventName;
			return ret;
		}

		private string? GetOpcodeName(EventOpcode opcode, string eventName)
		{
			switch (opcode)
			{
				case EventOpcode.Info:
					return "win:Info";

				case EventOpcode.Start:
					return "win:Start";

				case EventOpcode.Stop:
					return "win:Stop";

				case EventOpcode.DataCollectionStart:
					return "win:DC_Start";

				case EventOpcode.DataCollectionStop:
					return "win:DC_Stop";

				case EventOpcode.Extension:
					return "win:Extension";

				case EventOpcode.Reply:
					return "win:Reply";

				case EventOpcode.Resume:
					return "win:Resume";

				case EventOpcode.Suspend:
					return "win:Suspend";

				case EventOpcode.Send:
					return "win:Send";

				case EventOpcode.Receive:
					return "win:Receive";
			}

			if (opcodeTab == null || !opcodeTab.TryGetValue((int)opcode, out string? ret))
			{
				ManifestError(SR.Format(SR.EventSource_UndefinedOpcode, opcode, eventName), true);
				ret = null;
			}

			return ret;
		}

		private void AppendKeywords(StringBuilder sb, ulong keywords, string eventName)
		{
#if FEATURE_MANAGED_ETW_CHANNELS

            // ignore keywords associate with channels
            // See ValidPredefinedChannelKeywords def for more.
            keywords &= ~ValidPredefinedChannelKeywords;
#endif

			bool appended = false;
			for (ulong bit = 1; bit != 0; bit <<= 1)
			{
				if ((keywords & bit) != 0)
				{
					string? keyword = null;
					if ((keywordTab == null || !keywordTab.TryGetValue(bit, out keyword)) &&
						(bit >= (ulong)0x1000000000000))
					{
						// do not report Windows reserved keywords in the manifest (this allows the code
						// to be resilient to potential renaming of these keywords)
						keyword = string.Empty;
					}
					if (keyword == null)
					{
						ManifestError(SR.Format(SR.EventSource_UndefinedKeyword, "0x" + bit.ToString("x", CultureInfo.CurrentCulture), eventName), true);
						keyword = string.Empty;
					}

					if (keyword.Length != 0)
					{
						if (appended)
						{
							sb.Append(' ');
						}

						sb.Append(keyword);
						appended = true;
					}
				}
			}
		}

		private string GetTypeName(Type type)
		{
			if (type.IsEnum)
			{
				string typeName = GetTypeName(type.GetEnumUnderlyingType());
				return typeName.Replace("win:Int", "win:UInt"); // ETW requires enums to be unsigned.
			}

			switch (Type.GetTypeCode(type))
			{
				case TypeCode.Boolean:
					return "win:Boolean";

				case TypeCode.Byte:
					return "win:UInt8";

				case TypeCode.Char:
				case TypeCode.UInt16:
					return "win:UInt16";

				case TypeCode.UInt32:
					return "win:UInt32";

				case TypeCode.UInt64:
					return "win:UInt64";

				case TypeCode.SByte:
					return "win:Int8";

				case TypeCode.Int16:
					return "win:Int16";

				case TypeCode.Int32:
					return "win:Int32";

				case TypeCode.Int64:
					return "win:Int64";

				case TypeCode.String:
					return "win:UnicodeString";

				case TypeCode.Single:
					return "win:Float";

				case TypeCode.Double:
					return "win:Double";

				case TypeCode.DateTime:
					return "win:FILETIME";

				default:
					if (type == typeof(Guid))
						return "win:GUID";
					else if (type == typeof(IntPtr))
						return "win:Pointer";
					else if ((type.IsArray || type.IsPointer) && type.GetElementType() == typeof(byte))
						return "win:Binary";

					ManifestError(SR.Format(SR.EventSource_UnsupportedEventTypeInManifest, type.Name), true);
					return string.Empty;
			}
		}

		private static void UpdateStringBuilder([NotNull] ref StringBuilder? stringBuilder, string eventMessage, int startIndex, int count)
		{
			stringBuilder ??= new StringBuilder();
			stringBuilder.Append(eventMessage, startIndex, count);
		}

		private static readonly string[] s_escapes = { "&amp;", "&lt;", "&gt;", "&apos;", "&quot;", "%r", "%n", "%t" };

		// Manifest messages use %N conventions for their message substitutions.   Translate from
		// .NET conventions.   We can't use RegEx for this (we are in mscorlib), so we do it 'by hand'
		private string TranslateToManifestConvention(string eventMessage, string evtName)
		{
			StringBuilder? stringBuilder = null;        // We lazily create this
			int writtenSoFar = 0;
			for (int i = 0; ;)
			{
				if (i >= eventMessage.Length)
				{
					if (stringBuilder == null)
						return eventMessage;
					UpdateStringBuilder(ref stringBuilder, eventMessage, writtenSoFar, i - writtenSoFar);
					return stringBuilder.ToString();
				}

				int chIdx;
				if (eventMessage[i] == '%')
				{
					// handle format message escaping character '%' by escaping it
					UpdateStringBuilder(ref stringBuilder, eventMessage, writtenSoFar, i - writtenSoFar);
					stringBuilder.Append("%%");
					i++;
					writtenSoFar = i;
				}
				else if (i < eventMessage.Length - 1 &&
					(eventMessage[i] == '{' && eventMessage[i + 1] == '{' || eventMessage[i] == '}' && eventMessage[i + 1] == '}'))
				{
					// handle C# escaped '{" and '}'
					UpdateStringBuilder(ref stringBuilder, eventMessage, writtenSoFar, i - writtenSoFar);
					stringBuilder.Append(eventMessage[i]);
					i++; i++;
					writtenSoFar = i;
				}
				else if (eventMessage[i] == '{')
				{
					int leftBracket = i;
					i++;
					int argNum = 0;
					while (i < eventMessage.Length && char.IsDigit(eventMessage[i]))
					{
						argNum = argNum * 10 + eventMessage[i] - '0';
						i++;
					}
					if (i < eventMessage.Length && eventMessage[i] == '}')
					{
						i++;
						UpdateStringBuilder(ref stringBuilder, eventMessage, writtenSoFar, leftBracket - writtenSoFar);
						int manIndex = TranslateIndexToManifestConvention(argNum, evtName);
						stringBuilder.Append('%').Append(manIndex);

						// An '!' after the insert specifier {n} will be interpreted as a literal.
						// We'll escape it so that mc.exe does not attempt to consider it the
						// beginning of a format string.
						if (i < eventMessage.Length && eventMessage[i] == '!')
						{
							i++;
							stringBuilder.Append("%!");
						}
						writtenSoFar = i;
					}
					else
					{
						ManifestError(SR.Format(SR.EventSource_UnsupportedMessageProperty, evtName, eventMessage));
					}
				}
				else if ((chIdx = "&<>'\"\r\n\t".IndexOf(eventMessage[i])) >= 0)
				{
					UpdateStringBuilder(ref stringBuilder, eventMessage, writtenSoFar, i - writtenSoFar);
					i++;
					stringBuilder.Append(s_escapes[chIdx]);
					writtenSoFar = i;
				}
				else
					i++;
			}
		}

		private int TranslateIndexToManifestConvention(int idx, string evtName)
		{
			if (perEventByteArrayArgIndices.TryGetValue(evtName, out List<int>? byteArrArgIndices))
			{
				foreach (int byArrIdx in byteArrArgIndices)
				{
					if (idx >= byArrIdx)
						++idx;
					else
						break;
				}
			}
			return idx + 1;
		}

#if FEATURE_MANAGED_ETW_CHANNELS
        private sealed class ChannelInfo
        {
            public string? Name;
            public ulong Keywords;
            public EventChannelAttribute? Attribs;
        }
#endif

		private readonly Dictionary<int, string> opcodeTab;
		private Dictionary<int, string>? taskTab;
#if FEATURE_MANAGED_ETW_CHANNELS
        private Dictionary<int, ChannelInfo>? channelTab;
#endif
		private Dictionary<ulong, string>? keywordTab;
		private Dictionary<string, Type>? mapsTab;
		private readonly Dictionary<string, string> stringTab;       // Maps unlocalized strings to localized ones

#if FEATURE_MANAGED_ETW_CHANNELS

        // WCF used EventSource to mimic a existing ETW manifest.   To support this
        // in just their case, we allowed them to specify the keywords associated
        // with their channels explicitly.   ValidPredefinedChannelKeywords is
        // this set of channel keywords that we allow to be explicitly set.  You
        // can ignore these bits otherwise.
        internal const ulong ValidPredefinedChannelKeywords = 0xF000000000000000;
        private ulong nextChannelKeywordBit = 0x8000000000000000;   // available Keyword bit to be used for next channel definition, grows down
        private const int MaxCountChannels = 8; // a manifest can defined at most 8 ETW channels
#endif

		private readonly StringBuilder sb;               // Holds the provider information.
		private readonly StringBuilder events;           // Holds the events.
		private readonly StringBuilder templates;

#if FEATURE_MANAGED_ETW_CHANNELS
        private readonly string providerName;
#endif
		private readonly ResourceManager? resources;      // Look up localized strings here.
		private readonly EventManifestOptions flags;
		private readonly IList<string> errors;           // list of currently encountered errors
		private readonly Dictionary<string, List<int>> perEventByteArrayArgIndices;  // "event_name" -> List_of_Indices_of_Byte[]_Arg

		// State we track between StartEvent and EndEvent.
		private string? eventName;               // Name of the event currently being processed.

		private int numParams;                  // keeps track of the number of args the event has.
		private List<int>? byteArrArgIndices;   // keeps track of the index of each byte[] argument

		#endregion private
	}

	/// <summary>
	/// Used to send the m_rawManifest into the event dispatcher as a series of events.
	/// </summary>
	internal struct ManifestEnvelope
	{
		public const int MaxChunkSize = 0xFF00;

		public enum ManifestFormats : byte
		{
			SimpleXmlFormat = 1,          // simply dump the XML manifest as UTF8
		}

#if FEATURE_MANAGED_ETW
        public ManifestFormats Format;
        public byte MajorVersion;
        public byte MinorVersion;
        public byte Magic;
        public ushort TotalChunks;
        public ushort ChunkNumber;
#endif
	}

	#endregion private classes
}
