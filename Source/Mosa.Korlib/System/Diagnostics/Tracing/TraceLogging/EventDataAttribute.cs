// Copyright (c) MOSA Project. Licensed under the New BSD License.
#nullable enable

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Diagnostics.Tracing
{
	/// <summary>
	/// Used when authoring types that will be passed to EventSource.Write.
	/// EventSource.Write&lt;T> only works when T is either an anonymous type
	/// or a type with an [EventData] attribute. In addition, the properties
	/// of T must be supported property types. Supported property types include
	/// simple built-in types (int, string, Guid, DateTime, DateTimeOffset,
	/// KeyValuePair, etc.), anonymous types that only contain supported types,
	/// types with an [EventData] attribute, arrays of the above, and IEnumerable
	/// of the above.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, Inherited = false)]
	public class EventDataAttribute
		: Attribute
	{
		/// <summary>
		/// Gets or sets the name to use if this type is used for an
		/// implicitly-named event or an implicitly-named property.
		///
		/// Example 1:
		///
		///     EventSource.Write(null, new T()); // implicitly-named event
		///
		/// The name of the event will be determined as follows:
		///
		/// if (T has an EventData attribute and attribute.Name != null)
		///     eventName = attribute.Name;
		/// else
		///     eventName = typeof(T).Name;
		///
		/// Example 2:
		///
		///     EventSource.Write(name, new { _1 = new T() }); // implicitly-named field
		///
		/// The name of the field will be determined as follows:
		///
		/// if (T has an EventData attribute and attribute.Name != null)
		///     fieldName = attribute.Name;
		/// else
		///     fieldName = typeof(T).Name;
		/// </summary>
		public string? Name
		{
			get;
			set;
		}
	}
}
