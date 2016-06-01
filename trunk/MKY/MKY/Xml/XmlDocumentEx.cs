//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at https://sourceforge.net/projects/y-a-terminal/.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Author$
// $Date$
// $Revision$
// ------------------------------------------------------------------------------------------------
// MKY Development Version 1.0.14
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// See release notes for product version details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2016 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Collections.Generic;
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
		public static XmlDocument FromReader(XmlReader reader)
		{
			XmlDocument document = new XmlDocument();
			document.Load(reader);
			return (document);
		}

		/// <summary>
		/// Creates and returns an object tree of the given type from a document.
		/// </summary>
		public static object ToObjectTree(XmlDocument document, Type type)
		{
			// Save the resulting document into a string:
			StringBuilder sb = new StringBuilder();
			using (XmlWriter writer = XmlWriter.Create(sb)) // Unlike file serialization, string serialization will be UTF-16 encoded!
			{                                               // Use dedicated XML writer to e.g. preserve whitespace!
				document.Save(writer);
			}

			// Deserialize that string into an object tree:
			using (StringReader sr = new StringReader(sb.ToString()))
			{
				using (XmlReader xr = XmlReader.Create(sr)) // Use dedicated XML reader to e.g. preserve whitespace!
				{
					XmlSerializer serializer = new XmlSerializer(type);
					return (serializer.Deserialize(xr));
				}
			}
		}

		/// <summary>
		/// Creates and returns the default document of the given type.
		/// </summary>
		/// <param name="type">The type to be used.</param>
		public static XmlDocument CreateDefaultDocument(Type type)
		{
			return (CreateDefaultDocument(type, null));
		}

		/// <summary>
		/// Creates and returns the default document of the given type.
		/// </summary>
		/// <param name="type">The type to be used.</param>
		/// <param name="requiredSchema">Schema(s) required in addition to the default schema.</param>
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
				throw (new NotSupportedException("Interfaces cannot be serialized!"));
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
			StringBuilder sb = new StringBuilder();
			using (XmlWriter xw = XmlWriter.Create(sb)) // Unlike file serialization, string serialization will be UTF-16 encoded!
			{                                           // Use dedicated XML writer to e.g. preserve whitespace!
				XmlSerializer serializer = new XmlSerializer(type);
				serializer.Serialize(xw, obj);
			}

			// Load that string into an XML document that serves as base for new documents:
			XmlDocument defaultDocument = new XmlDocument();
			defaultDocument.LoadXml(sb.ToString());

			// Retrieve default schema of the given type:
			XmlReflectionImporter reflectionImporter = new XmlReflectionImporter();
			XmlTypeMapping typeMapping = reflectionImporter.ImportTypeMapping(type);
			XmlSchemas schemas = new XmlSchemas();
			XmlSchemaExporter schemaExporter = new XmlSchemaExporter(schemas);
			schemaExporter.ExportTypeMapping(typeMapping);

			// Add required additional schemas:
			if (requiredSchema != null)
			{
				foreach (XmlSchema s in requiredSchema)
					defaultDocument.Schemas.Add(s);
			}

			// Add schema of the given type:
			defaultDocument.Schemas.Add(schemas[0]);

			// Compile and validate the schemas:
			defaultDocument.Schemas.Compile();
			defaultDocument.Validate(null);

			return (defaultDocument);
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
