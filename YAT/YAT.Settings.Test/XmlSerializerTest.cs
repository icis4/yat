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
// YAT 2.0 Beta 4 Candidate 3 Development Version 1.99.31
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// See release notes for product version details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2013 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// YAT is licensed under the GNU LGPL.
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

using NUnit.Framework;

using YAT.Model.Settings;
using YAT.Model.Types;
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

		#region Tear Down Fixture
		//==========================================================================================
		// Tear Down Fixture
		//==========================================================================================

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "TearDown", Justification = "Naming according to NUnit.")]
		[TestFixtureTearDown]
		public virtual void TestFixtureTearDown()
		{
			Temp.CleanTempPath(GetType());
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
			string filePath = "";

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
			string filePath = "";

			filePath = Temp.MakeTempFilePath(GetType(), "RecentFileSettings", FileExtension);

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
			string filePath = "";

			filePath = Temp.MakeTempFilePath(GetType(), "PredefinedCommandPage", FileExtension);
			PredefinedCommandPage pcp = new PredefinedCommandPage();
			pcp.Commands.Add(new Command("Hello", "World"));
			pcp.Commands.Add(new Command("Hallo", "Wält"));
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
			string filePath = "";

			filePath = Temp.MakeTempFilePath(GetType(), "ExplicitSettings", FileExtension);
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
			string filePath = "";

			filePath = Temp.MakeTempFilePath(GetType(), "ImplicitSettings", FileExtension);
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
