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
// YAT 2.0 Gamma 2 Development Version 1.99.35
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// See release notes for product version details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2007-2015 Matthias Kläy.
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

using MKY.Diagnostics;
using MKY.IO;
using MKY.Settings;
using MKY.Xml.Schema;

using NUnit.Framework;

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
		private static readonly SettingsFilePaths StaticPaths = new SettingsFilePaths("!-Current");

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
			ArchiveSchema (StaticPaths.Path, "LocalUserSettingsSchema",  document);
			ArchiveDefault(StaticPaths.Path, "LocalUserSettingsDefault", document);
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
			// Terminal settings may rely on properly loaded applications settings.
			SelectiveTestSetUp();

			XmlDocument document = CreateSchemaAndDefaultDocument(typeof(TerminalSettingsRoot));
			ArchiveSchema (StaticPaths.Path, "TerminalSettingsSchema",  document);
			ArchiveDefault(StaticPaths.Path, "TerminalSettingsDefault", document);

			SelectiveTestTearDown();
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
			// Workspace settings may rely on properly loaded applications settings.
			SelectiveTestSetUp();

			XmlDocument document = CreateSchemaAndDefaultDocument(typeof(WorkspaceSettingsRoot));
			ArchiveSchema (StaticPaths.Path, "WorkspaceSettingsSchema",  document);
			ArchiveDefault(StaticPaths.Path, "WorkspaceSettingsDefault", document);

			SelectiveTestTearDown();
		}

		#endregion

		#endregion

		#region Private Methods
		//==========================================================================================
		// Private Methods
		//==========================================================================================

		private static void SelectiveTestSetUp()
		{
			// Create temporary in-memory application settings for this test run.
			ApplicationSettings.Create(ApplicationSettingsFileAccess.None);

			// Prevent auto-save of workspace settings.
			ApplicationSettings.LocalUserSettings.General.AutoSaveWorkspace = false;
		}

		private static void SelectiveTestTearDown()
		{
			// Close temporary in-memory application settings.
			ApplicationSettings.Close();
		}

		private static XmlDocument CreateSchemaAndDefaultDocument(Type type)
		{
			ConstructorInfo ci = type.GetConstructor(new Type[] { });
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
			defaultDocument.Schemas.Add(XmlSchemaEx.GuidSchema);
			defaultDocument.Schemas.Add(schemas[0]);
			defaultDocument.Schemas.Compile();
			defaultDocument.Validate(null);

			return (defaultDocument);
		}

		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Ensure that operation succeeds in any case.")]
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

				// Attention:
				// All YAT settings schema consists of two schemas:
				//  > A GUID extension to http://microsoft.com/wsdl/types/ (~ 1 kB)
				//  > The settings schema itself (>> 1 kB)
				// However, the order of the archived schemas is random! Therefore, a size check
				// is done here, and the files are swapped if needed. This ensures that file commits
				// to SVN will not result in unnecessary diffs.
				if (n >= 2)
				{
					DirectoryInfo di = new DirectoryInfo(path);
					FileInfo[] fis = di.GetFiles(fileName + "-?.xsd");

					if (fis.Length >= 2)
					{
						if (fis[0].Length > fis[1].Length)
						{
							string filePath0 = path + fileName + "-0.xsd";
							string filePath1 = path + fileName + "-1.xsd";

							PathEx.SwapExistingFiles(filePath0, filePath1);
						}
					}
				}
			}
			catch (Exception ex)
			{
				TraceEx.WriteException(typeof(SettingsFilesArchiver), ex);

				// Attention: The following call throws an exception, code after the call will not be executed.
				Assert.Fail("XML serialize error: " + ex.Message);
			}
		}

		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Ensure that operation succeeds in any case.")]
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

				// Attention: The following call throws an exception, code after the call will not be executed.
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
