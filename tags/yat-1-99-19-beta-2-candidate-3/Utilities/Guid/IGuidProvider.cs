using System;
using System.Collections.Generic;
using System.Text;

namespace MKY.Utilities.Guid
{
	/// <summary>
	/// Interface that can be implemented by types providing a <see cref="Guid"/>.
	/// </summary>
	public interface IGuidProvider
	{
		/// <summary>
		/// Returns the <see cref="Guid"/> of the providing object.
		/// </summary>
		System.Guid Guid { get; }
	}
}
