using System;
using System.Collections.Generic;
using System.Text;

namespace MKY.Utilities.Xml
{
	/// <summary>
	/// Interface for XML contents that have alternate XML elements, e.g. when an XML element
	/// has changed its name.
	/// </summary>
	public interface IAlternateXmlElementProvider
	{
		/// <summary>
		/// Alternate XML elements. Applies to any kind of XML nodes.
		/// </summary>
		AlternateXmlElement[] AlternateXmlElements { get; }
	}
}
