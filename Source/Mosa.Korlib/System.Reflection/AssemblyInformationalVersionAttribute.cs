// Copyright (c) MOSA Project. Licensed under the New BSD License.

namespace System.Reflection
{
	[AttributeUsage(AttributeTargets.Assembly, Inherited = false)]
	public sealed class AssemblyInformationalVersionAttribute : Attribute
	{
		private readonly string informationalVersion;

		/// <summary>
		/// The assembly informationalVersion.
		/// </summary>
		public string InformationalVersion
		{
			get { return informationalVersion; }
		}

		/// <summary>
		/// Initializes a new instance of the AssemblyInformationalVersionAttribute class.
		/// </summary>
		/// <param name="informationalVersion">The assembly informationalVersion.</param>
		public AssemblyInformationalVersionAttribute(string informationalVersion)
		{
			this.informationalVersion = informationalVersion;
		}
	}
}
