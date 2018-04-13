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
// MKY Version 1.0.26 Development
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
using System.Reflection;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace MKY.Xml
{
	/// <summary>
	/// XML document extensions.
	/// </summary>
	[SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix", Justification = "'Ex' emphasizes that it's an extension to an existing class and not a replacement as '2' would emphasize.")]
	public static class XmlDocumentEx
	{
		/// <summary>
		/// Reads XML input stream into a document.
		/// </summary>
		[SuppressMessage("Microsoft.Design", "CA1059:MembersShouldNotExposeCertainConcreteTypes", MessageId = "System.Xml.XmlNode", Justification = "Emphasize type.")]
		public static XmlDocument FromReader(XmlReader reader)
		{
			XmlDocument document = new XmlDocument();
			document.Load(reader);
			return (document);
		}

		/// <summary>
		/// Creates and returns an object tree of the given type from a document.
		/// </summary>
		[SuppressMessage("Microsoft.Design", "CA1059:MembersShouldNotExposeCertainConcreteTypes", MessageId = "System.Xml.XmlNode", Justification = "Emphasize type.")]
		public static object ToObjectTree(XmlDocument document, Type type)
		{
			// Save the resulting document into a string:
			var sb = new StringBuilder();
			using (var xw = XmlWriter.Create(sb)) // Unlike file serialization, string serialization will be UTF-16 encoded!
			{                                     // Use dedicated XML writer to e.g. preserve whitespace in XML content!
				document.Save(xw);
			}

			// Deserialize that string into an object tree:
			using (var sr = new StringReader(sb.ToString()))
			{
				using (var xr = XmlReader.Create(sr)) // Use dedicated XML reader to e.g. preserve whitespace in XML content!
				{
					var serializer = new XmlSerializer(type);
					return (serializer.Deserialize(xr));
				}
			}
		}

		/// <summary>
		/// Creates and returns the default document of the given type.
		/// </summary>
		/// <param name="type">The type to be used.</param>
		/// <param name="requiredSchema">Schema(s) required in addition to the default schema.</param>
		[SuppressMessage("Microsoft.Design", "CA1059:MembersShouldNotExposeCertainConcreteTypes", MessageId = "System.Xml.XmlNode", Justification = "Emphasize type.")]
		public static XmlDocument CreateDefaultDocument(Type type, params XmlSchema[] requiredSchema)
		{
			// Create an empty object tree of the type to be able to serialize it afterwards.
			object obj;
			if (type.IsValueType)
			{
				obj = Activator.CreateInstance(type);
			}
			else if (type.IsInterface)
			{
				throw (new NotSupportedException(MessageHelper.InvalidExecutionPreamble + "Interfaces cannot be serialized!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
			}
			else // IsClass
			{
				if (type.IsArray)
				{
					obj = Array.CreateInstance(type.GetElementType(), 0);
				}
				else
				{
					ConstructorInfo ci = type.GetConstructor(new Type[] { });
					if (ci != null)
						obj = ci.Invoke(new object[] { });
					else
						throw (new NotImplementedException("Type '" + type + "'does not provide a default constructor!"));
				}
			}

			// Serialize the empty object tree into a string:
			var sb = new StringBuilder();
			using (var xw = XmlWriter.Create(sb)) // Unlike file serialization, string serialization will be UTF-16 encoded!
			{                                     // Use dedicated XML writer to e.g. preserve whitespace in XML content!
				var serializer = new XmlSerializer(type);
				serializer.Serialize(xw, obj);
			}

			// Load that string into an XML document that serves as base for new documents:
			var defaultDocument = new XmlDocument();
			defaultDocument.PreserveWhitespace = true;
			defaultDocument.LoadXml(sb.ToString());
			defaultDocument.PreserveWhitespace = false;

			// Retrieve default schema of the given type:
			var reflectionImporter = new XmlReflectionImporter();
			var typeMapping = reflectionImporter.ImportTypeMapping(type);
			var schemas = new XmlSchemas();
			var schemaExporter = new XmlSchemaExporter(schemas);
			schemaExporter.ExportTypeMapping(typeMapping);

			// Add required additional schemas:
			if (requiredSchema != null)
			{
				foreach (var schema in requiredSchema)
					defaultDocument.Schemas.Add(schema);
			}

			// Add schema of the given type:
			defaultDocument.Schemas.Add(schemas[0]);

			// Compile and validate the schemas:
			defaultDocument.Schemas.Compile();
			defaultDocument.Validate(null);

			return (defaultDocument);
		}

		/// <summary>
		/// Writes the given XML document to the given path and file name.
		/// </summary>
		/// <param name="document">The document.</param>
		/// <param name="path">The path.</param>
		/// <param name="fileNameWithoutExtension">Name of the intended file.</param>
		/// <param name="fileExtension">Extension of the file.</param>
		[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Default parameters may result in cleaner code and clearly indicate the default behavior.")]
		[SuppressMessage("Microsoft.Design", "CA1059:MembersShouldNotExposeCertainConcreteTypes", MessageId = "System.Xml.XmlNode", Justification = "Well, 'XmlDocument.Schemas' is needed, 'IXPathNavigable' doesn't provide that member... Is this a bug in FxCop?")]
		public static void ToFile(XmlDocument document, string path, string fileNameWithoutExtension, string fileExtension = ".xml")
		{
			string filePath = path + fileNameWithoutExtension + (!string.IsNullOrEmpty(fileExtension) ? fileExtension : "");
			using (var sw = new StreamWriter(filePath, false, Encoding.UTF8))
			{
				var xws = new XmlWriterSettings();
				xws.Indent = true;

				using (var xw = XmlWriter.Create(sw, xws)) // Use dedicated XML writer to e.g. preserve whitespace in XML content!
				{
					document.Save(xw);
				}
			}
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
