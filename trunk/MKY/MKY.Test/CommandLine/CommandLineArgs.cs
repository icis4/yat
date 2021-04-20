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
// MKY Version 1.0.30
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
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "A", Justification = "Make test idea more obvious.")]
		A,

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "Bb", Justification = "Make test idea more obvious.")]
		Bb,

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Ccc", Justification = "Make test idea more obvious.")]
		Ccc
	}

	/// <summary></summary>
	[SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:FieldsMustBePrivate", Justification = "Command line arguments based on 'MKY.CommandLine.ArgsHandler' must be public.")]
	public class CommandLineArgs : ArgsHandler
	{
		private const string VisibilitySuppressionJustification = "Command line arguments based on 'MKY.CommandLine.ArgsHandler' must be public.";
		private const string NamingSuppressionJustification = "The field/property shall clearly state what it represents.";

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = VisibilitySuppressionJustification)]
		[ValueArg(Description = "A pure value argument.")]
		public string PureValueArg;

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = VisibilitySuppressionJustification)]
		[ValueArg(Description = "A combined value/option argument, this is the value argument description.")]
		[OptionArg(Name = "CombinedValueOptionArg", ShortName = "cvoa", Description = "A combined value/option argument, this is the option argument description.")]
		public string CombinedValueOptionArg;

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = VisibilitySuppressionJustification)]
		[SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "string", Justification = NamingSuppressionJustification)]
		[OptionArg(Name = "StringValueOption", ShortName = "svo", Description = "A string value option.")]
		public string StringValueOption;

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = VisibilitySuppressionJustification)]
		[SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "bool", Justification = NamingSuppressionJustification)]
		[OptionArg(Name = "BooleanOption", ShortName = "bo", Description = "A boolean option.")]
		public bool BooleanOption;

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = VisibilitySuppressionJustification)]
		[SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "int", Justification = NamingSuppressionJustification)]
		[OptionArg(Name = "IntValueOption", ShortName = "ivo", Description = "A int value option.")]
		public int IntValueOption;

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = VisibilitySuppressionJustification)]
		[SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "double", Justification = NamingSuppressionJustification)]
		[OptionArg(Name = "DoubleValueOption", ShortName = "dvo", Description = "A double value option.")]
		public double DoubleValueOption;

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = VisibilitySuppressionJustification)]
		[SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "enum", Justification = NamingSuppressionJustification)]
		[OptionArg(Name = "EnumValueOption", ShortName = "evo", Description = "An enum value option.")]
		public CommandLineEnum EnumValueOption;

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = VisibilitySuppressionJustification)]
		[SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "string", Justification = NamingSuppressionJustification)]
		[CLSCompliant(false)]
		[OptionArg(Name = "StringArrayOption", ShortNames = new string[] { "sao" }, Description = "A string array option.")]
		public string[] StringArrayOption;

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = VisibilitySuppressionJustification)]
		[SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "int", Justification = NamingSuppressionJustification)]
		[CLSCompliant(false)]
		[OptionArg(Name = "IntArrayOption", ShortNames = new string[] { "iao" }, Description = "An int array option.")]
		public int[] IntArrayOption;

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = VisibilitySuppressionJustification)]
		[SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "long", Justification = NamingSuppressionJustification)]
		[OptionArg(Name = "LongDescription", ShortName = "ld", Description =
			"A long description string value option that comes with a description that blablabla blablabla blablabla blablabla blablabla " +
			"blablabla blablabla blablabla blablabla blablabla blablabla blablabla blablabla blablabla." + EnvironmentEx.NewLineConstWorkaround +
			"blablabla blablabla blablabla blablabla blablabla blablabla blablabla blablabla blablabla blablabla blablabla blablabla " +
			"blablabla blablabla blablabla blablabla blablabla blablabla blablabla blablabla blablabla blablabla blablabla blablabla.")]
		public string LongDescription;

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = VisibilitySuppressionJustification)]
		[CLSCompliant(false)]
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
