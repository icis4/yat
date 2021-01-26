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
// YAT Version 2.4.0
// ------------------------------------------------------------------------------------------------
// See release notes for product version details.
// See SVN change log for file revision details.
// Author(s): Matthias Klaey
// ------------------------------------------------------------------------------------------------
// Copyright © 2007-2021 Matthias Kläy.
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
		#region Tests
		//==========================================================================================
		// Tests
		//==========================================================================================

		#region Tests > TestEmptyThenSingleLineText
		//------------------------------------------------------------------------------------------
		// Tests > TestEmptyThenSingleLineText
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		[Test]
		public virtual void TestEmptyThenSingleLineText()
		{
			var c = new Command();

			string text = "ABC";
			c.SingleLineText = text;
			AssertSingleLineText(c, text);

			c.Description = ""; // Valid but to be ignored.
			AssertSingleLineText(c, text);

			string description = "XYZ";
			c.Description = description;
			AssertSingleLineText(c, text, description);
		}

		#endregion

		#region Tests > TestEmptyThenDescriptionThenSingleLineText
		//------------------------------------------------------------------------------------------
		// Tests > TestEmptyThenDescriptionThenSingleLineText
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		[Test]
		public virtual void TestEmptyThenDescriptionThenSingleLineText()
		{
			var c = new Command();

			string description = "XYZ";
			c.Description = description;
			AssertDescription(c, null, description);

			string text = "ABC";
			c.SingleLineText = text;
			AssertSingleLineText(c, text, description);
		}

		#endregion

		#region Tests > TestInitialSingleLineTextThenModify
		//------------------------------------------------------------------------------------------
		// Tests > TestInitialSingleLineTextThenModify
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		[Test]
		public virtual void TestInitialSingleLineTextThenModify()
		{
			string initialText = "ABC";
			var c = new Command(initialText);
			AssertSingleLineText(c, initialText);

			string modifiedText = "abc"; // Change casing.
			c.SingleLineText = modifiedText;
			AssertSingleLineText(c, modifiedText);

			modifiedText = "abcdef"; // Add text.
			c.SingleLineText = modifiedText;
			AssertSingleLineText(c, modifiedText);

			modifiedText = "abef"; // Remove text.
			c.SingleLineText = modifiedText;
			AssertSingleLineText(c, modifiedText);

			c.Description = ""; // Valid but to be ignored.
			AssertSingleLineText(c, modifiedText);

			string description = "XYZ";
			c.Description = description;
			AssertSingleLineText(c, modifiedText, description);

			modifiedText = "abc";
			c.SingleLineText = modifiedText;
			AssertSingleLineText(c, modifiedText, description);

			modifiedText = "ABC";
			c.SingleLineText = modifiedText;
			AssertSingleLineText(c, modifiedText, description);
		}

		#endregion

		#region Tests > TestEmptyThenMultiLineText
		//------------------------------------------------------------------------------------------
		// Tests > TestEmptyThenMultiLineText
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		[Test]
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Multi", Justification = "What's wrong with 'MultiLine'?")]
		public virtual void TestEmptyThenMultiLineText()
		{
			var c = new Command();

			string[] text = new string[] { "ABC", "DEF" };
			string singleLineText = "<2 lines...> [ABC] [DEF]";
			c.MultiLineText = text;
			AssertMultiLineText(c, text, singleLineText);

			c.Description = ""; // Valid but to be ignored.
			AssertMultiLineText(c, text, singleLineText);

			string description = "XYZ";
			c.Description = description;
			AssertMultiLineText(c, text, singleLineText, description);
		}

		#endregion

		#region Tests > TestEmptyThenDescriptionThenMultiLineText
		//------------------------------------------------------------------------------------------
		// Tests > TestEmptyThenDescriptionThenMultiLineText
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		[Test]
		public virtual void TestEmptyThenDescriptionThenMultiLineText()
		{
			var c = new Command();

			string description = "XYZ";
			c.Description = description;
			AssertDescription(c, null, description);

			string[] text = new string[] { "ABC", "DEF" };
			string singleLineText = "<2 lines...> [ABC] [DEF]";
			c.MultiLineText = text;
			AssertMultiLineText(c, text, singleLineText, description);
		}

		#endregion

		#region Tests > TestInitialMultiLineTextThenModify
		//------------------------------------------------------------------------------------------
		// Tests > TestInitialMultiLineTextThenModify
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		[Test]
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Multi", Justification = "What's wrong with 'MultiLine'?")]
		public virtual void TestInitialMultiLineTextThenModify()
		{
			string[] initialMulti = new string[] { "ABC", "DEF" };
			string initialSingle = "<2 lines...> [ABC] [DEF]";
			var c = new Command(initialMulti);
			AssertMultiLineText(c, initialMulti, initialSingle);

			string[] modifiedMulti = new string[] { "ABC", "def" }; // Change casing.
			string modifiedSingle = "<2 lines...> [ABC] [def]";
			c.MultiLineText = modifiedMulti;
			AssertMultiLineText(c, modifiedMulti, modifiedSingle);

			modifiedMulti = new string[] { "ABC", "def", "ghi" }; // Add a line.
			modifiedSingle = "<3 lines...> [ABC] [def] [ghi]";
			c.MultiLineText = modifiedMulti;
			AssertMultiLineText(c, modifiedMulti, modifiedSingle);

			modifiedMulti = new string[] { "ABC", "ghi" }; // Remove a line.
			modifiedSingle = "<2 lines...> [ABC] [ghi]";
			c.MultiLineText = modifiedMulti;
			AssertMultiLineText(c, modifiedMulti, modifiedSingle);

			c.Description = ""; // Valid but to be ignored.
			AssertMultiLineText(c, modifiedMulti, modifiedSingle);

			string description = "XYZ";
			c.Description = description;
			AssertMultiLineText(c, modifiedMulti, modifiedSingle, description);

			modifiedMulti = new string[] { "ABC", "DEF" };
			modifiedSingle = "<2 lines...> [ABC] [DEF]";
			c.MultiLineText = modifiedMulti;
			AssertMultiLineText(c, modifiedMulti, modifiedSingle, description);
		}

		#endregion

		#endregion

		#region Assert
		//==========================================================================================
		// Assert
		//==========================================================================================

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Default parameters may result in cleaner code and clearly indicate the default behavior.")]
		protected virtual void AssertDescription(Command c, string text, string description, Domain.Parser.Mode modes = Domain.Parser.Mode.Default)
		{
			if (!string.IsNullOrEmpty(text))
				AssertSingleLineText(c, text, description, modes);
			else
				Assert.That(c.Description, Is.EqualTo(description));
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Default parameters may result in cleaner code and clearly indicate the default behavior.")]
		protected virtual void AssertSingleLineText(Command c, string text, string description = null, Domain.Parser.Mode modes = Domain.Parser.Mode.Default)
		{
			Assert.That((text != null) || (description != null)); // Precondition for this assertion.

			Assert.That(c.IsDefined,          Is.True);
			Assert.That(c.IsValid(modes),     Is.True);
			Assert.That(c.Caption,            Is.EqualTo(text));

			Assert.That(c.IsText,             Is.True);
			Assert.That(c.IsValidText(modes), Is.True);
			Assert.That(c.IsSingleLineText,   Is.True);
			Assert.That(c.SingleLineText,     Is.EqualTo(text));

			Assert.That(c.IsPartialTextEol,   Is.False);
			Assert.That(c.IsPartialText,      Is.False);
			Assert.That(c.PartialText,        Is.EqualTo(text));

			Assert.That(c.IsMultiLineText,    Is.False);
			Assert.That(c.MultiLineText,      Is.EquivalentTo(new string[] { text }));

			Assert.That(c.IsFilePath,         Is.False);
			Assert.That(c.IsValidFilePath(),  Is.False);
			Assert.That(c.FilePath,           Is.Null.Or.Empty);

			if (string.IsNullOrEmpty(description))
				Assert.That(c.Description,    Is.EqualTo(text));
			else
				Assert.That(c.Description,    Is.EqualTo(description));
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Default parameters may result in cleaner code and clearly indicate the default behavior.")]
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "multi", Justification = "What's wrong with 'MultiLine'?")]
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Multi", Justification = "What's wrong with 'MultiLine'?")]
		protected virtual void AssertMultiLineText(Command c, string[] multiLineText, string singleLineText, string description = null, Domain.Parser.Mode modes = Domain.Parser.Mode.Default)
		{
			Assert.That(((multiLineText != null) || (singleLineText != null)) || (description != null)); // Precondition for this assertion.

			Assert.That(c.IsDefined,          Is.True);
			Assert.That(c.IsValid(modes),     Is.True);
			Assert.That(c.Caption,            Is.EqualTo(singleLineText));

			Assert.That(c.IsText,             Is.True);
			Assert.That(c.IsValidText(modes), Is.True);
			Assert.That(c.IsSingleLineText,   Is.False);
			Assert.That(c.SingleLineText,     Is.EqualTo(singleLineText));

			Assert.That(c.IsPartialTextEol,   Is.False);
			Assert.That(c.IsPartialText,      Is.False);
			Assert.That(c.PartialText,        Is.EqualTo(singleLineText));

			Assert.That(c.IsMultiLineText,    Is.True);
			Assert.That(c.MultiLineText,      Is.EqualTo(multiLineText));

			Assert.That(c.IsFilePath,         Is.False);
			Assert.That(c.IsValidFilePath(),  Is.False);
			Assert.That(c.FilePath,           Is.Null.Or.Empty);

			if (string.IsNullOrEmpty(description))
				Assert.That(c.Description,    Is.EqualTo(singleLineText));
			else
				Assert.That(c.Description,    Is.EqualTo(description));
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
