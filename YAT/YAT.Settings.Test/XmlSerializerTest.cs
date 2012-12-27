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
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

using MKY.Diagnostics;
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
			TestSerialization(filePath, typeof(Command[]), a);

			filePath = Temp.MakeTempFilePath(GetType(), "EmptyListOfCommands", FileExtension);
			List<Command> l = new List<Command>();
			TestSerialization(filePath, typeof(List<Command>), l);
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
			TestSerialization(filePath, typeof(RecentFileSettings), rfs);
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
			TestSerialization(filePath, typeof(PredefinedCommandPage), pcp);

			PredefinedCommandPageCollection c = new PredefinedCommandPageCollection();
			c.Add(pcp);
			c.Add(pcp);

			filePath = Temp.MakeTempFilePath(GetType(), "PredefinedCommandSettings", FileExtension);
			PredefinedCommandSettings pcs = new PredefinedCommandSettings();
			pcs.Pages = c;
			TestSerialization(filePath, typeof(PredefinedCommandSettings), pcs);
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
			TestSerialization(filePath, typeof(ExplicitSettings), s);
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
			TestSerialization(filePath, typeof(ImplicitSettings), s);
		}

		#endregion

		#endregion

		#endregion

		#region Private Methods
		//==========================================================================================
		// Private Methods
		//==========================================================================================

		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Enusre that really all exceptions get caught.")]
		private static void TestSerialization(string filePath, Type type, object obj)
		{
			// Save.
			try
			{
				MKY.Xml.Serialization.XmlSerializerEx.SerializeToFile(filePath, type, obj);
			}
			catch (Exception ex)
			{
				TraceEx.WriteException(typeof(XmlSerializerTest), ex);

				// Attention: The following call throws an exception, code below that call won't be executed.
				Assert.Fail("XML serialize error: " + ex.Message);
			}

			// Load.
			try
			{
				MKY.Xml.Serialization.XmlSerializerEx.DeserializeFromFile(filePath, type);
			}
			catch (Exception ex)
			{
				TraceEx.WriteException(typeof(XmlSerializerTest), ex);

				// Attention: The following call throws an exception, code below that call won't be executed.
				Assert.Fail("XML deserialize error: " + ex.Message);
			}
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
