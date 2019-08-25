//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at http://sourceforge.net/projects/y-a-terminal.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Author$
// $Date$
// $Revision$
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// ------------------------------------------------------------------------------------------------
// Copyright � 2003-2004 HSR Hochschule f�r Technik Rapperswil.
// Copyright � 2003-2010 Matthias Kl�y.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.XPath;

namespace MKY.Xml
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

		#region Object Members

		/// <summary>
		/// Determines whether this instance and the specified object have value equality.
		/// </summary>
		public override bool Equals(object obj)
		{
			if (ReferenceEquals(obj, null))
				return (false);

			if (GetType() != obj.GetType())
				return (false);

			AlternateXmlElement other = (AlternateXmlElement)obj;
			return
			(
				(XmlPath             == other.XmlPath) &&
				(LocalName           == other.LocalName) &&
				(AlternateLocalNames == other.AlternateLocalNames)
			);
		}

		/// <summary></summary>
		public override int GetHashCode()
		{
			return
			(
				XmlPath            .GetHashCode() ^
				LocalName          .GetHashCode() ^
				AlternateLocalNames.GetHashCode()
			);
		}

		#endregion

		#region Comparison Operators

		/// <summary>
		/// Determines whether the two specified objects have reference or value equality.
		/// </summary>
		public static bool operator ==(AlternateXmlElement lhs, AlternateXmlElement rhs)
		{
			// Value type implementation of operator ==.
			// See MKY.Test.EqualityTest for details.

			if (ReferenceEquals(lhs, rhs)) return (true);
			if (ReferenceEquals(lhs, null)) return (false);
			if (ReferenceEquals(rhs, null)) return (false);

			return (lhs.Equals(rhs));
		}

		/// <summary>
		/// Determines whether the two specified objects have reference and value inequality.
		/// </summary>
		public static bool operator !=(AlternateXmlElement lhs, AlternateXmlElement rhs)
		{
			return (!(lhs == rhs));
		}

		#endregion
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
				if (!string.IsNullOrEmpty(localName))
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
				if (!string.IsNullOrEmpty(localName))
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
				if (Types.ArrayEx.ValuesEqual(element.XmlPath, standardXmlPath))
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