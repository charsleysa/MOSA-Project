// Copyright (c) MOSA Project. Licensed under the New BSD License.

namespace System.Reflection
{
	[AttributeUsage(AttributeTargets.Assembly, Inherited = false)]
	public sealed class AssemblyProductAttribute : Attribute
	{
		private readonly string product;

		/// <summary>
		/// The assembly product.
		/// </summary>
		public string Product
		{
			get { return product; }
		}

		/// <summary>
		/// Initializes a new instance of the AssemblyProductAttribute class.
		/// </summary>
		/// <param name="product">The assembly product.</param>
		public AssemblyProductAttribute(string product)
		{
			this.product = product;
		}
	}
}
