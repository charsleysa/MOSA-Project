// Copyright (c) MOSA Project. Licensed under the New BSD License.

namespace System.Reflection
{
	[AttributeUsage(AttributeTargets.Assembly, Inherited = false)]
	public sealed class AssemblyVersionAttribute : Attribute
	{
		private readonly string version;

		/// <summary>
		/// The assembly version.
		/// </summary>
		public string Version
		{
			get { return version; }
		}

		/// <summary>
		/// Initializes a new instance of the AssemblyVersionAttribute class.
		/// </summary>
		/// <param name="version">The assembly version.</param>
		public AssemblyVersionAttribute(string version)
		{
			this.version = version;
		}
	}
}
