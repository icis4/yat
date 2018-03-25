//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at https://sourceforge.net/projects/y-a-terminal/.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Revision$
// $Date$
// $Author$
// ------------------------------------------------------------------------------------------------
// MKY Version 1.0.25
// ------------------------------------------------------------------------------------------------
// See release notes for product version details.
// See SVN change log for file revision details.
// Author(s): Matthias Klaey
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2018 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Schema;

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
		[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "StyleCop isn't able to skip URLs...")]
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
			using (var sr = new StringReader(GuidSchemaString))
				GuidSchema = XmlSchema.Read(sr, ValidationCallback);
		}

		private static void ValidationCallback(object sender, ValidationEventArgs args)
		{
			if (args.Severity == XmlSeverityType.Error)
				throw (new InvalidOperationException(MessageHelper.InvalidExecutionPreamble + "Validation of the XML schema for a GUID should never have failed!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
		}

		/// <summary>
		/// Writes the XML schema of the default document of the given XML document to the given path and file name.
		/// </summary>
		/// <remarks>
		/// If the document contains multiple schemas, multiple files will be saved.
		/// </remarks>
		/// <param name="type">The type.</param>
		/// <param name="path">The path.</param>
		/// <param name="intendedFileNameWithoutExtension">Name of the intended file.</param>
		/// <param name="fileExtension">Extension of the file.</param>
		[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Default parameters may result in cleaner code and clearly indicate the default behavior.")]
		public static void ToFile(Type type, string path, string intendedFileNameWithoutExtension, string fileExtension = ".xsd")
		{
			var document = XmlDocumentEx.CreateDefaultDocument(type);
			ToFile(document, path, intendedFileNameWithoutExtension, fileExtension);
		}

		/// <summary>
		/// Writes the XML schema of the given XML document to the given path and file name.
		/// </summary>
		/// <remarks>
		/// If the document contains multiple schemas, multiple files will be saved.
		/// </remarks>
		/// <param name="document">The document.</param>
		/// <param name="path">The path.</param>
		/// <param name="intendedFileNameWithoutExtension">Name of the intended file.</param>
		/// <param name="fileExtension">Extension of the file.</param>
		[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Default parameters may result in cleaner code and clearly indicate the default behavior.")]
		[SuppressMessage("Microsoft.Design", "CA1059:MembersShouldNotExposeCertainConcreteTypes", MessageId = "System.Xml.XmlNode", Justification = "Well, 'XmlDocument.Schemas' is needed, 'IXPathNavigable' doesn't provide that member... Is this a bug in FxCop?")]
		public static void ToFile(XmlDocument document, string path, string intendedFileNameWithoutExtension, string fileExtension = ".xsd")
		{
			int count = document.Schemas.Schemas().Count;
			int index = 0;
			foreach (XmlSchema schema in document.Schemas.Schemas())
			{
				ToFile(schema, path, intendedFileNameWithoutExtension, count, index, fileExtension);

				index++;
			}
		}

		/// <summary>
		/// Writes the given XML schema to the given path and file name.
		/// </summary>
		/// <remarks>
		/// If the number of schemas is greater than 1, the effective file name will be postfixed with <paramref name="index"/>.
		/// </remarks>
		/// <param name="schema">The schema.</param>
		/// <param name="path">The path.</param>
		/// <param name="intendedFileNameWithoutExtension">Name of the intended file.</param>
		/// <param name="count">The number of schemas to save in total.</param>
		/// <param name="index">The index of the current schema to save.</param>
		/// <param name="fileExtension">Extension of the file.</param>
		[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "'postfix' is a correct English term and 'postfixed' seems the obvious participle.")]
		[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Default parameters may result in cleaner code and clearly indicate the default behavior.")]
		public static void ToFile(XmlSchema schema, string path, string intendedFileNameWithoutExtension, int count = 1, int index = 0, string fileExtension = ".xsd")
		{
			string effectiveFilePath;
			if (count <= 1)
				effectiveFilePath = path + Path.DirectorySeparatorChar + intendedFileNameWithoutExtension + (!string.IsNullOrEmpty(fileExtension) ? fileExtension : "");
			else
				effectiveFilePath = path + Path.DirectorySeparatorChar + intendedFileNameWithoutExtension + "-" + index + (!string.IsNullOrEmpty(fileExtension) ? fileExtension : "");

			using (var sw = new StreamWriter(effectiveFilePath, false, Encoding.UTF8))
			{
				var xws = new XmlWriterSettings();
				xws.Indent = true;

				using (var xw = XmlWriter.Create(sw, xws)) // Use dedicated XML writer to e.g. preserve whitespace in XML content!
				{
					schema.Write(xw);
				}
			}
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
