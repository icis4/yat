//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at http://sourceforge.net/projects/y-a-terminal/.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Author$
// $Date$
// $Revision$
// ------------------------------------------------------------------------------------------------
// MKY Version 1.0.9
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// See release notes for product version details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2013 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace MKY.Xml.Schema
{
	/// <summary>
	/// XML schema extensions.
	/// </summary>
	[SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix", Justification = "'Ex' emphasizes that it's an extension to an existing class and not a replacement as '2' would emphasize.")]
	public static class XmlSchemaEx
	{
		/// <summary>
		/// GUID serialization by default does work, but compiling or saving a schema containing a
		/// GUID throws an exception (type 'http://microsoft.com/wsdl/types/:guid' is not declared)
		/// as the GUID type somehow got forgotten in the default /wsdl/types. This XML schema
		/// provides the missing schema.
		/// </summary>
		/// <remarks>
		/// The code has be taken from the following link:
		/// http://stackoverflow.com/questions/687884/what-is-the-correct-way-of-using-the-guid-type-in-a-xsd-file
		/// 
		/// Saying hello to StyleCop ;-.
		/// </remarks>
		[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "StyleCop doesn't seem to be able to skip URLs...")]
		[SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "Why not, the .NET framework itself does it everywhere...")]
		public static readonly XmlSchema GuidSchema;

		private static readonly string GuidSchemaString =
			@"<?xml version=""1.0"" encoding=""utf-8""?>"                                    + Environment.NewLine +
			@"<xs:schema xmlns:xs=""http://www.w3.org/2001/XMLSchema"""                      + Environment.NewLine +
			@"    targetNamespace=""http://microsoft.com/wsdl/types/"" >"                    + Environment.NewLine +
			@"    <xs:simpleType name=""guid"">"                                             + Environment.NewLine +
			@"        <xs:annotation>"                                                       + Environment.NewLine +
			@"            <xs:documentation xml:lang=""en"">"                                + Environment.NewLine +
			@"                The representation of a GUID, generally the id of an element." + Environment.NewLine +
			@"            </xs:documentation>"                                               + Environment.NewLine +
			@"        </xs:annotation>"                                                      + Environment.NewLine +
			@"        <xs:restriction base=""xs:string"">"                                   + Environment.NewLine +
			@"            <xs:pattern value=""[0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{12}""/>" + Environment.NewLine +
			@"        </xs:restriction>"                                                     + Environment.NewLine +
			@"    </xs:simpleType>"                                                          + Environment.NewLine +
			@"</xs:schema>";

		/// <summary></summary>
		[SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline", Justification = "'GuidSchemaString' must be readonly because it refers to Environment.NewLine.")]
		static XmlSchemaEx()
		{
			StringReader sr = new StringReader(GuidSchemaString);
			GuidSchema = XmlSchema.Read(sr, ValidationCallback);
		}

		private static void ValidationCallback(object sender, ValidationEventArgs args)
		{
			if (args.Severity == XmlSeverityType.Error)
				throw (new InvalidOperationException("Validation of the XML schema for a GUID should never have failed"));
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
