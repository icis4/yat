//==================================================================================================
// $URL$
// $Author$
// $Date$
// $Revision$
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2010 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
// ------------------------------------------------------------------------------------------------
// Visit YAT at http://sourceforge.net/projects/y-a-terminal.
// Contact YAT by mailto:maettu_this@users.sourceforge.net.
//==================================================================================================

using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.XPath;

namespace MKY.Utilities.Xml
{
	/// <summary></summary>
	public struct AlternateXmlElement
	{
		/// <summary></summary>
		public string[] XmlPath;

		/// <summary></summary>
		public string LocalName;

		/// <summary></summary>
		public string[] AlternateLocalNames;

		/// <summary></summary>
		public AlternateXmlElement(string[] xmlPath, string localName, string[] alternateLocalNames)
		{
			XmlPath = xmlPath;
			LocalName = localName;
			AlternateLocalNames = alternateLocalNames;
		}
	}

	/// <summary>
	/// Extends <see cref="TolerantXmlSerializer"/> such that it is capable to replace XML nodes
	/// by nodes with an alternate name.
	/// </summary>
	public class AlternateTolerantXmlSerializer : TolerantXmlSerializer
	{
		#region Fields
		//==========================================================================================
		// Fields
		//==========================================================================================

		private AlternateXmlElement[] alternates;

		#endregion

		#region Object Lifetime
		//==========================================================================================
		// Object Lifetime
		//==========================================================================================

		/// <summary></summary>
		public AlternateTolerantXmlSerializer(Type type, AlternateXmlElement[] alternates)
			: base (type)
		{
			this.alternates = alternates;
		}

		#endregion

		#region Overridden Protected Methods
		//==========================================================================================
		// Overridden Protected Methods
		//==========================================================================================

		/// <summary>
		/// Tries to match a given input attribute to the given output attribute.
		/// </summary>
		/// <remarks>
		/// <see cref="TolerantXmlSerializer.CopyTolerantly"/> on why input must be matched to output and not vice-versa.
		/// </remarks>
		protected override bool TryToMatchAttribute(XPathNavigator inputNavigator, ref XPathNavigator outputNavigator)
		{
			// try standard match
			if (base.TryToMatchAttribute(inputNavigator, ref outputNavigator))
				return (true);
			
			// no success, try alternate match
			XmlNode inputNode = inputNavigator.UnderlyingObject as XmlNode;
			if (inputNode != null)
			{
				string localName = GetLocalNameAlternateTolerant(GetXmlPath(inputNode), inputNavigator.Name, this.alternates);
				if ((localName != null) && (localName.Length > 0))
				{
					if (outputNavigator.MoveToAttribute(localName, inputNavigator.NamespaceURI))
						return (true);
				}
			}

			// still no success
			return (false);
		}

		/// <summary>
		/// Tries to match a given input child to the given output child.
		/// </summary>
		/// <remarks>
		/// <see cref="TolerantXmlSerializer.CopyTolerantly"/> on why input must be matched to output and not vice-versa.
		/// </remarks>
		protected override bool TryToMatchChild(XPathNavigator inputNavigator, ref XPathNavigator outputNavigator)
		{
			// try standard match
			if (base.TryToMatchChild(inputNavigator, ref outputNavigator))
				return (true);

			// no success, try alternate match
			XmlNode inputNode = inputNavigator.UnderlyingObject as XmlNode;
			if (inputNode != null)
			{
				string localName = GetLocalNameAlternateTolerant(GetXmlPath(inputNode), inputNavigator.Name, this.alternates);
				if ((localName != null) && (localName.Length > 0))
				{
					if (outputNavigator.MoveToChild(localName, inputNavigator.NamespaceURI))
						return (true);
				}
			}

			// still no success
			return (false);
		}

		#endregion

		#region Private Methods
		//==========================================================================================
		// Private Methods
		//==========================================================================================

		private string[] GetXmlPath(XmlNode node)
		{
			XmlNode parent = node.ParentNode;
			List<string> xmlPath = new List<string>();
			while (parent != null)
			{
				xmlPath.Add(parent.Name);
				parent = parent.ParentNode;
			}
			xmlPath.Reverse();
			return (xmlPath.ToArray());
		}

		private string GetLocalNameAlternateTolerant(string[] standardXmlPath, string localName, AlternateXmlElement[] alternates)
		{
			foreach (AlternateXmlElement element in alternates)
			{
				// compare XML path
				if (Types.XArray.ValuesEqual(element.XmlPath, standardXmlPath))
				{
					// compare alternates to given local name
					foreach (string alternateLocalName in element.AlternateLocalNames)
					{
						if (alternateLocalName == localName)
							return (element.LocalName);
					}
				}
			}
			return (localName);
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
