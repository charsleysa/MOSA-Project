// Copyright (c) MOSA Project. Licensed under the New BSD License.

namespace System.Reflection
{
	[AttributeUsage(AttributeTargets.Assembly, Inherited = false)]
	public sealed class AssemblyFileVersionAttribute : Attribute
	{
		private readonly string fileVersion;

		/// <summary>
		/// The assembly fileVersion.
		/// </summary>
		public string FileVersion
		{
			get { return fileVersion; }
		}

		/// <summary>
		/// Initializes a new instance of the AssemblyFileVersionAttribute class.
		/// </summary>
		/// <param name="fileVersion">The assembly fileVersion.</param>
		public AssemblyFileVersionAttribute(string fileVersion)
		{
			this.fileVersion = fileVersion;
		}
	}
}
