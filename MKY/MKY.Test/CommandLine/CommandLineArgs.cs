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
using System.Diagnostics.CodeAnalysis;
using System.Text;

using MKY.CommandLine;

#endregion

namespace MKY.Test.CommandLine
{
	/// <summary></summary>
	[SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix", Justification = "Name must be different than namespace.")]
	public enum CommandLineEnum
	{
		/// <summary></summary>
		A,

		/// <summary></summary>
		Bb,

		/// <summary></summary>
		Ccc,
	}

	/// <summary></summary>
	public class CommandLineArgs : ArgsHandler
	{
		private const string SuppressionJustification = "This is a simple container for command line args.";

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = SuppressionJustification)]
		[ValueArg(Description = "A pure value argument.")]
		public string PureValueArg;

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = SuppressionJustification)]
		[ValueArg(Description = "A combined value/option argument, this is the value argument description.")]
		[OptionArg(Name = "CombinedValueOptionArg", ShortName = "cvoa", Description = "A combined value/option argument, this is the option argument description.")]
		public string CombinedValueOptionArg;

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = SuppressionJustification)]
		[OptionArg(Name = "StringValueOption", ShortName = "svo", Description = "A string value option.")]
		public string StringValueOption;

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = SuppressionJustification)]
		[OptionArg(Name = "BooleanOption", ShortName = "bo", Description = "A boolean option.")]
		public bool BooleanOption;

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = SuppressionJustification)]
		[OptionArg(Name = "IntValueOption", ShortName = "ivo", Description = "A int value option.")]
		public int IntValueOption;

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = SuppressionJustification)]
		[OptionArg(Name = "DoubleValueOption", ShortName = "dvo", Description = "A double value option.")]
		public double DoubleValueOption;

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = SuppressionJustification)]
		[OptionArg(Name = "EnumValueOption", ShortName = "evo", Description = "An enum value option.")]
		public CommandLineEnum EnumValueOption;

		/// <summary></summary>
		[CLSCompliant(false)]
		[SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = SuppressionJustification)]
		[OptionArg(Name = "StringArrayOption", ShortNames = new string[] { "sao" }, Description = "A string array option.")]
		public string[] StringArrayOption;

		/// <summary></summary>
		[CLSCompliant(false)]
		[SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = SuppressionJustification)]
		[OptionArg(Name = "IntArrayOption", ShortNames = new string[] { "iao" }, Description = "An int array option.")]
		public int[] IntArrayOption;

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = SuppressionJustification)]
		[OptionArg(Name = "LongDescription", ShortName = "ld", Description =
			"A long description string value option that comes with a description that blablabla blablabla blablabla blablabla blablabla " +
			"blablabla blablabla blablabla blablabla blablabla blablabla blablabla blablabla blablabla." + EnvironmentEx.NewLineConstWorkaround +
			"blablabla blablabla blablabla blablabla blablabla blablabla blablabla blablabla blablabla blablabla blablabla blablabla " +
			"blablabla blablabla blablabla blablabla blablabla blablabla blablabla blablabla blablabla blablabla blablabla blablabla.")]
		public string LongDescription;

		/// <summary></summary>
		[CLSCompliant(false)]
		[SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = SuppressionJustification)]
		[OptionArg(Names = new string[] { "Help", "HelpText" }, ShortNames = new string[] { "h", "?" }, Description = "Display this help text.")]
		public bool HelpIsRequested;

		/// <summary></summary>
		public CommandLineArgs(string[] args)
			: base(args, true, true, true)
		{
		}

		/// <summary>
		/// Gets the help text.
		/// </summary>
		public override string GetHelpText(int maxWidth)
		{
			StringBuilder helpText = new StringBuilder();

			helpText.AppendLine(                                "Usage:");
			helpText.AppendLine();
			helpText.Append(SplitIntoLines(maxWidth, MinorIndent, "Application[.exe] [<Values>] [<Options>]"));
			helpText.AppendLine();
			helpText.AppendLine();
			helpText.AppendLine(                                "Usage examples:");
			helpText.AppendLine();
			helpText.Append(SplitIntoLines(maxWidth, MinorIndent, "Application /svo=ABC"));
			helpText.Append(SplitIntoLines(maxWidth, MajorIndent,         "Run an eeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeendless sequence of actions."));
			helpText.AppendLine();

			helpText.Append(base.GetHelpText(maxWidth));

			helpText.AppendLine();
			helpText.Append(SplitIntoLines(maxWidth, MinorIndent, "Some additional notes."));
			helpText.AppendLine();
			return (helpText.ToString());
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
