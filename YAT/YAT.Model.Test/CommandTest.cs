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

using System.Diagnostics.CodeAnalysis;

using NUnit.Framework;

using YAT.Model.Types;

namespace YAT.Model.Test
{
	/// <summary></summary>
	[TestFixture]
	public class CommandTest
	{
		/// <summary></summary>
		[Test]
		public virtual void TestEmptyThenSingleLineText()
		{
			var c = new Command();

			const string Text = "ABC";
			c.SingleLineText = Text;
			AssertSingleLineText(c, Text);

			const string Description = "XYZ";
			c.Description = Description;
			AssertSingleLineText(c, Text, Description);
		}

		/// <summary></summary>
		[Test]
		public virtual void TestInitialSingleLineTextThenModify()
		{
			const string InitialText = "ABC";
			const string InitialDescription = "XYZ";
			var c = new Command(InitialDescription, InitialText);
			AssertSingleLineText(c, InitialText, InitialDescription);

			const string ModifiedDescription = "uvw";
			c.Description = ModifiedDescription;
			AssertSingleLineText(c, InitialText, ModifiedDescription);

			const string ModifiedText = "def";
			c.SingleLineText = ModifiedText;
			AssertSingleLineText(c, ModifiedText, ModifiedDescription);
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Default parameters may result in cleaner code and clearly indicate the default behavior.")]
		public virtual void AssertSingleLineText(Command c, string text, string description = null)
		{
			Assert.That(c.IsDefined,        Is.True);
			Assert.That(c.IsValid,          Is.True);
			Assert.That(c.Caption,          Is.EqualTo(text));

			Assert.That(c.IsText,           Is.True);
			Assert.That(c.IsValidText,      Is.True);
			Assert.That(c.IsSingleLineText, Is.True);
			Assert.That(c.SingleLineText,   Is.EqualTo(text));

			Assert.That(c.IsPartialTextEol, Is.False);
			Assert.That(c.IsPartialText,    Is.False);
			Assert.That(c.PartialText,      Is.EqualTo(text));

			Assert.That(c.IsMultiLineText,  Is.False);
			Assert.That(c.MultiLineText,    Is.EquivalentTo(new string[] { text }));

			Assert.That(c.IsFilePath,       Is.False);
			Assert.That(c.IsValidFilePath,  Is.False);
			Assert.That(c.FilePath,         Is.Null.Or.Empty);

			if (string.IsNullOrEmpty(description))
				Assert.That(c.Description,  Is.EqualTo(text));
			else
				Assert.That(c.Description,  Is.EqualTo(description));
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
