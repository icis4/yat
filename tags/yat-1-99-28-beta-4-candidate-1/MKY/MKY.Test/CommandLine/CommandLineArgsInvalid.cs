//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at http://sourceforge.net/projects/y-a-terminal.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Author$
// $Date$
// $Revision$
// ------------------------------------------------------------------------------------------------
// YAT 2.0 Beta 4 Candidate 1 Version 1.99.28
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// See release notes for product version details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2011 Matthias Kläy.
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
using System.Diagnostics.CodeAnalysis;
using System.Text;

using MKY.CommandLine;

#endregion

namespace MKY.Test.CommandLine
{
	/// <summary></summary>
	public class CommandLineArgsInvalid1 : ArgsHandler
	{
		private const string SuppressionJustification = "This is a simple container for command line args.";

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = SuppressionJustification)]
		[OptionArg(Name = "AAA", ShortName = "a")]
		public string AAA;

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = SuppressionJustification)]
		[OptionArg(Name = "BBB", ShortName = "a")]
		public string BBB;

		/// <summary></summary>
		public CommandLineArgsInvalid1(string[] args)
			: base(args)
		{
		}
	}

	/// <summary></summary>
	public class CommandLineArgsInvalid2 : ArgsHandler
	{
		private const string SuppressionJustification = "This is a simple container for command line args.";

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = SuppressionJustification)]
		[OptionArg(Name = "AAA", ShortName = "a")]
		public string AAA;

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = SuppressionJustification)]
		[OptionArg(Name = "AAA", ShortName = "b")]
		public string BBB;

		/// <summary></summary>
		public CommandLineArgsInvalid2(string[] args)
			: base(args)
		{
		}
	}

	/// <summary></summary>
	public class CommandLineArgsInvalid3 : ArgsHandler
	{
		private const string SuppressionJustification = "This is a simple container for command line args.";

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = SuppressionJustification)]
		[OptionArg(Name = "AAA", ShortName = "a")]
		public string AAA;

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = SuppressionJustification)]
		[OptionArg(Name = "AAA", ShortName = "a")]
		public string BBB;

		/// <summary></summary>
		public CommandLineArgsInvalid3(string[] args)
			: base(args)
		{
		}
	}

	/// <summary></summary>
	public class CommandLineArgsInvalid4 : ArgsHandler
	{
		private const string SuppressionJustification = "This is a simple container for command line args.";

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = SuppressionJustification)]
		[OptionArg(Name = "AAA", ShortName = "a")]
		public string AAA;

		/// <summary></summary>
		[CLSCompliant(false)]
		[SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = SuppressionJustification)]
		[OptionArg(Name = "BBB", ShortNames = new string[] { "b", "a" })]
		public string BBB;

		/// <summary></summary>
		public CommandLineArgsInvalid4(string[] args)
			: base(args)
		{
		}
	}

	/// <summary></summary>
	public class CommandLineArgsInvalid5 : ArgsHandler
	{
		private const string SuppressionJustification = "This is a simple container for command line args.";

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = SuppressionJustification)]
		[OptionArg(Name = "AAA", ShortName = "a")]
		public string AAA;

		/// <summary></summary>
		[CLSCompliant(false)]
		[SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = SuppressionJustification)]
		[OptionArg(Name = "BBB", ShortNames = new string[] { "a", "b" })]
		public string BBB;

		/// <summary></summary>
		public CommandLineArgsInvalid5(string[] args)
			: base(args)
		{
		}
	}

	/// <summary></summary>
	public class CommandLineArgsInvalid6 : ArgsHandler
	{
		private const string SuppressionJustification = "This is a simple container for command line args.";

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = SuppressionJustification)]
		[OptionArg(Name = "AAA", ShortName = "a")]
		public string AAA;

		/// <summary></summary>
		[CLSCompliant(false)]
		[SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = SuppressionJustification)]
		[OptionArg(Names = new string[] { "BBB", "AAA" }, ShortName = "b")]
		public string BBB;

		/// <summary></summary>
		public CommandLineArgsInvalid6(string[] args)
			: base(args)
		{
		}
	}

	/// <summary></summary>
	public class CommandLineArgsInvalid7 : ArgsHandler
	{
		private const string SuppressionJustification = "This is a simple container for command line args.";

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = SuppressionJustification)]
		[OptionArg(Name = "AAA", ShortName = "a")]
		public string AAA;

		/// <summary></summary>
		[CLSCompliant(false)]
		[SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = SuppressionJustification)]
		[OptionArg(Names = new string[] { "AAA", "BBB" }, ShortName = "b")]
		public string BBB;

		/// <summary></summary>
		public CommandLineArgsInvalid7(string[] args)
			: base(args)
		{
		}
	}

	/// <summary></summary>
	public class CommandLineArgsInvalid8 : ArgsHandler
	{
		private const string SuppressionJustification = "This is a simple container for command line args.";

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = SuppressionJustification)]
		[OptionArg(Name = "AAA", ShortName = "a")]
		public string AAA;

		/// <summary></summary>
		[CLSCompliant(false)]
		[SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = SuppressionJustification)]
		[OptionArg(Names = new string[] { "BBB", "AAA" }, ShortNames = new string[] { "b", "a" })]
		public string BBB;

		/// <summary></summary>
		public CommandLineArgsInvalid8(string[] args)
			: base(args)
		{
		}
	}

	/// <summary></summary>
	public class CommandLineArgsInvalid9 : ArgsHandler
	{
		private const string SuppressionJustification = "This is a simple container for command line args.";

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = SuppressionJustification)]
		[OptionArg(Name = "AAA", ShortName = "a")]
		public string AAA;

		/// <summary></summary>
		[CLSCompliant(false)]
		[SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = SuppressionJustification)]
		[OptionArg(Names = new string[] { "AAA", "BBB" }, ShortNames = new string[] { "a", "b" })]
		public string BBB;

		/// <summary></summary>
		public CommandLineArgsInvalid9(string[] args)
			: base(args)
		{
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
