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
// YAT 2.0 Gamma 2'' Version 1.99.52
// ------------------------------------------------------------------------------------------------
// See release notes for product version details.
// See SVN change log for file revision details.
// Author(s): Matthias Klaey
// ------------------------------------------------------------------------------------------------
// Copyright © 2007-2017 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

#region Using
//==================================================================================================
// Using
//==================================================================================================

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

using MKY.IO;
using MKY.Recent;
using MKY.Settings;

using NUnit.Framework;

using YAT.Model.Settings;
using YAT.Model.Types;
using YAT.Settings.Application;
using YAT.Settings.Terminal;

#endregion

namespace YAT.Settings.Test
{
	/// <summary></summary>
	[TestFixture]
	public class XmlSerializerTest
	{
		#region Constants
		//==========================================================================================
		// Constants
		//==========================================================================================

		private const string FileExtension = ".xml";

		#endregion

		#region TestFixture
		//==========================================================================================
		// TestFixture
		//==========================================================================================

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "SetUp", Justification = "Naming according to NUnit.")]
		[TestFixtureSetUp]
		public virtual void TestFixtureSetUp()
		{
			// Create temporary in-memory application settings for this test run:
			ApplicationSettings.Create(ApplicationSettingsFileAccess.None);

			// Prevent auto-save of workspace settings:
			ApplicationSettings.LocalUserSettings.General.AutoSaveWorkspace = false;
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "TearDown", Justification = "Naming according to NUnit.")]
		[TestFixtureTearDown]
		public virtual void TestFixtureTearDown()
		{
			Temp.CleanTempPath(GetType());

			// Close and dispose of temporary in-memory application settings:
			ApplicationSettings.CloseAndDispose();
		}

		#endregion

		#region Tests
		//==========================================================================================
		// Tests
		//==========================================================================================

		#region Tests > Serialization
		//------------------------------------------------------------------------------------------
		// Tests > Serialization
		//------------------------------------------------------------------------------------------

		#region Tests > Serialization > EmptyCommand
		//------------------------------------------------------------------------------------------
		// Tests > Serialization > EmptyCommand
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		[Test]
		public virtual void TestEmptyCommandSerialization()
		{
			string filePath;

			filePath = Temp.MakeTempFilePath(GetType(), "EmptyArrayOfCommands", FileExtension);
			Command[] a = new Command[] { };
			MKY.Test.Xml.Serialization.XmlSerializerTest.TestSerializationChain(filePath, typeof(Command[]), a);

			filePath = Temp.MakeTempFilePath(GetType(), "EmptyListOfCommands", FileExtension);
			List<Command> l = new List<Command>();
			MKY.Test.Xml.Serialization.XmlSerializerTest.TestSerializationChain(filePath, typeof(List<Command>), l);
		}

		#endregion

		#region Tests > Serialization > Recent
		//------------------------------------------------------------------------------------------
		// Tests > Serialization > Recent
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		[Test]
		public virtual void TestRecentSerialization()
		{
			string filePath = Temp.MakeTempFilePath(GetType(), "RecentFileSettings", FileExtension);

			RecentItemCollection<string> ric = new RecentItemCollection<string>();
			ric.Add(new RecentItem<string>("RIA"));
			ric.Add(new RecentItem<string>("RIB"));

			RecentFileSettings rfs = new RecentFileSettings();
			rfs.FilePaths = ric;
			MKY.Test.Xml.Serialization.XmlSerializerTest.TestSerializationChain(filePath, typeof(RecentFileSettings), rfs);
		}

		#endregion

		#region Tests > Serialization > Predefined
		//------------------------------------------------------------------------------------------
		// Tests > Serialization > Predefined
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		[Test]
		public virtual void TestPredefinedSerialization()
		{
			string filePath;

			filePath = Temp.MakeTempFilePath(GetType(), "PredefinedCommandPage", FileExtension);
			PredefinedCommandPage pcp = new PredefinedCommandPage();
			pcp.Commands.Add(new Command("Hello", new string[] { "World" }));
			pcp.Commands.Add(new Command("Hallo", new string[] { "Wält"  }));
			MKY.Test.Xml.Serialization.XmlSerializerTest.TestSerializationChain(filePath, typeof(PredefinedCommandPage), pcp);

			PredefinedCommandPageCollection c = new PredefinedCommandPageCollection();
			c.Add(pcp);
			c.Add(pcp);

			filePath = Temp.MakeTempFilePath(GetType(), "PredefinedCommandSettings", FileExtension);
			PredefinedCommandSettings pcs = new PredefinedCommandSettings();
			pcs.Pages = c;
			MKY.Test.Xml.Serialization.XmlSerializerTest.TestSerializationChain(filePath, typeof(PredefinedCommandSettings), pcs);
		}

		#endregion

		#region Tests > Serialization > Explicit
		//------------------------------------------------------------------------------------------
		// Tests > Serialization > Explicit
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		[Test]
		public virtual void TestExplicitSerialization()
		{
			string filePath = Temp.MakeTempFilePath(GetType(), "ExplicitSettings", FileExtension);
			ExplicitSettings s = new ExplicitSettings();
			MKY.Test.Xml.Serialization.XmlSerializerTest.TestSerializationChain(filePath, typeof(ExplicitSettings), s);
		}

		#endregion

		#region Tests > Serialization > Implicit
		//------------------------------------------------------------------------------------------
		// Tests > Serialization > Implicit
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		[Test]
		public virtual void TestImplicitSerialization()
		{
			string filePath = Temp.MakeTempFilePath(GetType(), "ImplicitSettings", FileExtension);
			ImplicitSettings s = new ImplicitSettings();
			MKY.Test.Xml.Serialization.XmlSerializerTest.TestSerializationChain(filePath, typeof(ImplicitSettings), s);
		}

		#endregion

		#endregion

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
