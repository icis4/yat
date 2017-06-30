﻿//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at https://sourceforge.net/projects/y-a-terminal/.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Revision$
// $Date$
// $Author$
// ------------------------------------------------------------------------------------------------
// MKY Version 1.0.19
// ------------------------------------------------------------------------------------------------
// See release notes for product version details.
// See SVN change log for file revision details.
// Author(s): Matthias Klaey
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2017 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

#region Using
//==================================================================================================
// Using
//==================================================================================================

using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.XPath;

#endregion

namespace MKY.Xml.Serialization
{
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
			: base(type)
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
			// Try standard match.
			if (base.TryToMatchAttribute(inputNavigator, ref outputNavigator))
				return (true);
			
			// No success, try alternate match.
			var inputNode = (inputNavigator.UnderlyingObject as XmlNode);
			if (inputNode != null)
			{
				string localName = GetLocalNameAlternateTolerant(GetXmlPath(inputNode), inputNavigator.Name, this.alternates);
				if (!string.IsNullOrEmpty(localName))
				{
					if (outputNavigator.MoveToAttribute(localName, inputNavigator.NamespaceURI))
						return (true);
				}
			}

			// Still no success.
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
			// Try standard match.
			if (base.TryToMatchChild(inputNavigator, ref outputNavigator))
				return (true);

			// No success, try alternate match.
			var inputNode = (inputNavigator.UnderlyingObject as XmlNode);
			if (inputNode != null)
			{
				string localName = GetLocalNameAlternateTolerant(GetXmlPath(inputNode), inputNavigator.Name, this.alternates);
				if (!string.IsNullOrEmpty(localName))
				{
					if (outputNavigator.MoveToChild(localName, inputNavigator.NamespaceURI))
						return (true);
				}
			}

			// Still no success.
			return (false);
		}

		#endregion

		#region Non-Public Methods
		//==========================================================================================
		// Non-Public Methods
		//==========================================================================================

		private static string[] GetXmlPath(XmlNode node)
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

		private static string GetLocalNameAlternateTolerant(string[] standardXmlPath, string localName, AlternateXmlElement[] alternates)
		{
			if (alternates != null)
			{
				foreach (AlternateXmlElement element in alternates)
				{
					// Compare XML path:
					if (ArrayEx.ElementsEqual(element.XmlPath, standardXmlPath))
					{
						// Compare alternates to given local name:
						foreach (string alternateLocalName in element.AlternateLocalNames)
						{
							if (StringEx.EqualsOrdinalIgnoreCase(alternateLocalName, localName))
								return (element.LocalName);
						}
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
