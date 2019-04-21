// Copyright (c) MOSA Project. Licensed under the New BSD License.

namespace System.Reflection
{
	[AttributeUsage(AttributeTargets.Assembly, Inherited = false)]
	public sealed class AssemblyCompanyAttribute : Attribute
	{
		private readonly string company;

		/// <summary>
		/// The assembly company.
		/// </summary>
		public string Company
		{
			get { return company; }
		}

		/// <summary>
		/// Initializes a new instance of the AssemblyCompanyAttribute class.
		/// </summary>
		/// <param name="company">The assembly company.</param>
		public AssemblyCompanyAttribute(string title)
		{
			this.company = title;
		}
	}
}
