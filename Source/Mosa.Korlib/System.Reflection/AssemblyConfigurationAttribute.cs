// Copyright (c) MOSA Project. Licensed under the New BSD License.

namespace System.Reflection
{
	[AttributeUsage(AttributeTargets.Assembly, Inherited = false)]
	public sealed class AssemblyConfigurationAttribute : Attribute
	{
		private readonly string configuration;

		/// <summary>
		/// The assembly configuration.
		/// </summary>
		public string Configuration
		{
			get { return configuration; }
		}

		/// <summary>
		/// Initializes a new instance of the AssemblyConfigurationAttribute class.
		/// </summary>
		/// <param name="configuration">The assembly configuration.</param>
		public AssemblyConfigurationAttribute(string configuration)
		{
			this.configuration = configuration;
		}
	}
}
