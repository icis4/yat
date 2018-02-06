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
// YAT 2.0 Almost Final Version 1.99.95
// ------------------------------------------------------------------------------------------------
// See release notes for product version details.
// See SVN change log for file revision details.
// Author(s): Matthias Klaey
// ------------------------------------------------------------------------------------------------
// Copyright © 2007-2018 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

#region Using
//==================================================================================================
// Using
//==================================================================================================

using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Xml;

using MKY.IO;
using MKY.Settings;
using MKY.Xml;
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

		/// <summary></summary>
		[Test]
		public virtual void ArchiveLocalUserSettings()
		{
			var document = XmlDocumentEx.CreateDefaultDocument(typeof(LocalUserSettingsRoot), XmlSchemaEx.GuidSchema); // GUID extension, for details see 'GuidSchema'.
			ArchiveSchema (document, StaticPaths.Path, "LocalUserSettingsSchema");
			ArchiveDefault(document, StaticPaths.Path, "LocalUserSettingsDefault");
		}

		/// <summary></summary>
		[Test]
		public virtual void ArchiveRoamingUserSettings()
		{
			var document = XmlDocumentEx.CreateDefaultDocument(typeof(RoamingUserSettingsRoot), XmlSchemaEx.GuidSchema); // GUID extension, for details see 'GuidSchema'.
			ArchiveSchema (document, StaticPaths.Path, "RoamingUserSettingsSchema");
			ArchiveDefault(document, StaticPaths.Path, "RoamingUserSettingsDefault");
		}

		/// <summary></summary>
		[Test]
		public virtual void ArchiveTerminalSettings()
		{
			// Terminal settings may rely on properly loaded applications settings.
			SelectiveTestSetUp();

			var document = XmlDocumentEx.CreateDefaultDocument(typeof(TerminalSettingsRoot), XmlSchemaEx.GuidSchema); // GUID extension, for details see 'GuidSchema'.
			ArchiveSchema (document, StaticPaths.Path, "TerminalSettingsSchema");
			ArchiveDefault(document, StaticPaths.Path, "TerminalSettingsDefault");

			SelectiveTestTearDown();
		}

		/// <summary></summary>
		[Test]
		public virtual void ArchiveWorkspaceSettings()
		{
			// Workspace settings may rely on properly loaded applications settings.
			SelectiveTestSetUp();

			var document = XmlDocumentEx.CreateDefaultDocument(typeof(WorkspaceSettingsRoot), XmlSchemaEx.GuidSchema); // GUID extension, for details see 'GuidSchema'.
			ArchiveSchema (document, StaticPaths.Path, "WorkspaceSettingsSchema");
			ArchiveDefault(document, StaticPaths.Path, "WorkspaceSettingsDefault");

			SelectiveTestTearDown();
		}

		#endregion

		#region Non-Public Methods
		//==========================================================================================
		// Non-Public Methods
		//==========================================================================================

		private static void SelectiveTestSetUp()
		{
			// Create temporary in-memory application settings for this test run:
			ApplicationSettings.Create(ApplicationSettingsFileAccess.None);

			// Prevent auto-save of workspace settings:
			ApplicationSettings.LocalUserSettings.General.AutoSaveWorkspace = false;
		}

		private static void SelectiveTestTearDown()
		{
			// Close and dispose of temporary in-memory application settings:
			ApplicationSettings.CloseAndDispose();
		}

		private static void ArchiveSchema(XmlDocument document, string path, string intendedFileNameWithoutExtension)
		{
			XmlSchemaEx.ToFile(document, path, intendedFileNameWithoutExtension);

			// Attention:
			// All YAT settings schema consists of two schemas:
			//  > A GUID extension to http://microsoft.com/wsdl/types/ (~ 1 kB)
			//  > The settings schema itself (>> 1 kB)
			// However, the order of the archived schemas is random! Therefore, a size check
			// is done here, and the files are swapped if needed. This ensures that file commits
			// to SVN will not result in unnecessary diffs.
			if (document.Schemas.Schemas().Count >= 2)
			{
				DirectoryInfo di = new DirectoryInfo(path);
				FileInfo[] fis = di.GetFiles(intendedFileNameWithoutExtension + "-?.xsd");

				if (fis.Length >= 2)
				{
					if (fis[0].Length > fis[1].Length)
					{
						string filePath0 = path + intendedFileNameWithoutExtension + "-0.xsd";
						string filePath1 = path + intendedFileNameWithoutExtension + "-1.xsd";

						FileEx.Swap(filePath0, filePath1);
					}
				}
			}
		}

		private static void ArchiveDefault(XmlDocument document, string path, string fileName)
		{
			XmlDocumentEx.ToFile(document, path, fileName);
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
