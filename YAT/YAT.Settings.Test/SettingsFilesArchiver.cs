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
// YAT 2.0 Beta 4 Candidate 2 Development Version 1.99.29
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// See release notes for product version details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2012 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// YAT is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

#region Using
//==================================================================================================
// Using
//==================================================================================================

using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

using NUnit.Framework;

using MKY.Diagnostics;
using MKY.Xml.Schema;

using YAT.Settings.Application;
using YAT.Settings.Terminal;
using YAT.Settings.Workspace;

#endregion

namespace YAT.Settings.Test
{
	/// <summary></summary>
	[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Archiver", Justification = "'Archiver' is a correct English term.")]
	[TestFixture, Explicit("This test fixture has the sole purpose to perform storing of settings schemas and defaults to the YAT settings archive")]
	public class SettingsFilesArchiver
	{
		#region Static Fields
		//==========================================================================================
		// Static Fields
		//==========================================================================================

		/// <summary></summary>
		private static readonly SettingsFilePaths staticPaths = new SettingsFilePaths();

		#endregion

		#region Archive
		//==========================================================================================
		// Archive
		//==========================================================================================

		#region Archive > LocalUserSettings
		//------------------------------------------------------------------------------------------
		// Archive > LocalUserSettings
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		[Test]
		public virtual void ArchiveLocalUserSettings()
		{
			XmlDocument document = CreateSchemaAndDefaultDocument(typeof(LocalUserSettingsRoot));
			ArchiveSchema (staticPaths.Path, "LocalUserSettingsSchema",  document);
			ArchiveDefault(staticPaths.Path, "LocalUserSettingsDefault", document);
		}

		#endregion

		#region Archive > TerminalSettings
		//------------------------------------------------------------------------------------------
		// Archive > TerminalSettings
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		[Test]
		public virtual void ArchiveTerminalSettings()
		{
			XmlDocument document = CreateSchemaAndDefaultDocument(typeof(TerminalSettingsRoot));
			ArchiveSchema (staticPaths.Path, "TerminalSettingsSchema",  document);
			ArchiveDefault(staticPaths.Path, "TerminalSettingsDefault", document);
		}

		#endregion

		#region Archive > WorkspaceSettings
		//------------------------------------------------------------------------------------------
		// Archive > WorkspaceSettings
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		[Test]
		public virtual void ArchiveWorkspaceSettings()
		{
			XmlDocument document = CreateSchemaAndDefaultDocument(typeof(WorkspaceSettingsRoot));
			ArchiveSchema (staticPaths.Path, "WorkspaceSettingsSchema",  document);
			ArchiveDefault(staticPaths.Path, "WorkspaceSettingsDefault", document);
		}

		#endregion

		#endregion

		#region Private Methods
		//==========================================================================================
		// Private Methods
		//==========================================================================================

		private static XmlDocument CreateSchemaAndDefaultDocument(Type type)
		{
			ConstructorInfo ci = type.GetConstructor(new System.Type[] { });
			object obj = ci.Invoke(new object[] { });

			// Serialize the empty object tree into a string.
			// Unlike file serialization, this string serialization will be UTF-16 encoded.
			StringBuilder sb = new StringBuilder();
			System.Xml.XmlWriter writer = System.Xml.XmlWriter.Create(sb);
			XmlSerializer serializer = new XmlSerializer(type);
			serializer.Serialize(writer, obj);

			// Load that string into an XML document that serves as base for new documents.
			XmlDocument defaultDocument = new XmlDocument();
			defaultDocument.LoadXml(sb.ToString());

			// Retrieve default schema.
			XmlReflectionImporter reflectionImporter = new XmlReflectionImporter();
			XmlTypeMapping typeMapping = reflectionImporter.ImportTypeMapping(type);
			XmlSchemas schemas = new XmlSchemas();
			XmlSchemaExporter schemaExporter = new XmlSchemaExporter(schemas);
			schemaExporter.ExportTypeMapping(typeMapping);

			// Set and compile default schema.
			defaultDocument.Schemas.Add(schemas[0]);
			defaultDocument.Schemas.Add(XmlSchemaEx.GuidSchema);
			defaultDocument.Schemas.Compile();
			defaultDocument.Validate(null);

			return (defaultDocument);
		}

		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Enusre that really all exceptions get caught.")]
		private static void ArchiveSchema(string path, string fileName, XmlDocument document)
		{
			try
			{
				int n = document.Schemas.Schemas().Count;
				int i = 0;
				foreach (XmlSchema schema in document.Schemas.Schemas())
				{
					string filePath;
					if (n <= 1)
						filePath = path + fileName + ".xsd";
					else
						filePath = path + fileName + "-" + i + ".xsd";

					using (StreamWriter sw = new StreamWriter(filePath, false, Encoding.UTF8))
					{
						schema.Write(sw);
					}
					Trace.WriteLine
					(
						"For archiving purposes, schema written to" + Environment.NewLine +
						@"""" + filePath + @""""
					);
					i++;
				}
			}
			catch (Exception ex)
			{
				TraceEx.WriteException(typeof(SettingsFilesArchiver), ex);

				// Attention: The following call throws an exception, code below that call won't be executed.
				Assert.Fail("XML serialize error: " + ex.Message);
			}
		}

		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Enusre that really all exceptions get caught.")]
		private static void ArchiveDefault(string path, string fileName, XmlDocument document)
		{
			try
			{
				string filePath = path + fileName + ".xml";
				using (StreamWriter sw = new StreamWriter(filePath, false, Encoding.UTF8))
				{
					document.Save(sw);
				}
				Trace.WriteLine
				(
					"For archiving purposes, default written to" + Environment.NewLine +
					@"""" + filePath + @""""
				);
			}
			catch (Exception ex)
			{
				TraceEx.WriteException(typeof(SettingsFilesArchiver), ex);

				// Attention: The following call throws an exception, code below that call won't be executed.
				Assert.Fail("XML serialize error: " + ex.Message);
			}
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
