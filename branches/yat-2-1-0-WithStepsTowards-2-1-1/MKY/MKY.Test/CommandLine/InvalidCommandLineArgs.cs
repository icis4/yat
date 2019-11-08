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
// MKY Version 1.0.28 Development
// ------------------------------------------------------------------------------------------------
// See release notes for product version details.
// See SVN change log for file revision details.
// Author(s): Matthias Klaey
// ------------------------------------------------------------------------------------------------
// Copyright © 2007-2019 Matthias Kläy.
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
	[SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:FieldsMustBePrivate", Justification = "Command line arguments based on 'MKY.CommandLine.ArgsHandler' must be public.")]
	public class InvalidCommandLineArgs1 : ArgsHandler
	{
		private const string VisibilitySuppressionJustification = "Command line arguments based on 'MKY.CommandLine.ArgsHandler' must be public.";

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = VisibilitySuppressionJustification)]
		[OptionArg(Name = "AAA", ShortName = "a")]
		public string AAA;

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = VisibilitySuppressionJustification)]
		[OptionArg(Name = "BBB", ShortName = "a")]
		public string BBB;

		/// <summary></summary>
		public InvalidCommandLineArgs1(string[] args)
			: base(args, false, true, false)
		{
		}
	}

	/// <summary></summary>
	[SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:FieldsMustBePrivate", Justification = "Command line arguments based on 'MKY.CommandLine.ArgsHandler' must be public.")]
	public class InvalidCommandLineArgs2 : ArgsHandler
	{
		private const string VisibilitySuppressionJustification = "Command line arguments based on 'MKY.CommandLine.ArgsHandler' must be public.";

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = VisibilitySuppressionJustification)]
		[OptionArg(Name = "AAA", ShortName = "a")]
		public string AAA;

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = VisibilitySuppressionJustification)]
		[OptionArg(Name = "AAA", ShortName = "b")]
		public string BBB;

		/// <summary></summary>
		public InvalidCommandLineArgs2(string[] args)
			: base(args, false, true, false)
		{
		}
	}

	/// <summary></summary>
	[SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:FieldsMustBePrivate", Justification = "Command line arguments based on 'MKY.CommandLine.ArgsHandler' must be public.")]
	public class InvalidCommandLineArgs3 : ArgsHandler
	{
		private const string VisibilitySuppressionJustification = "Command line arguments based on 'MKY.CommandLine.ArgsHandler' must be public.";

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = VisibilitySuppressionJustification)]
		[OptionArg(Name = "AAA", ShortName = "a")]
		public string AAA;

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = VisibilitySuppressionJustification)]
		[OptionArg(Name = "AAA", ShortName = "a")]
		public string BBB;

		/// <summary></summary>
		public InvalidCommandLineArgs3(string[] args)
			: base(args, false, true, false)
		{
		}
	}

	/// <summary></summary>
	[SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:FieldsMustBePrivate", Justification = "Command line arguments based on 'MKY.CommandLine.ArgsHandler' must be public.")]
	public class InvalidCommandLineArgs4 : ArgsHandler
	{
		private const string VisibilitySuppressionJustification = "Command line arguments based on 'MKY.CommandLine.ArgsHandler' must be public.";

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = VisibilitySuppressionJustification)]
		[OptionArg(Name = "AAA", ShortName = "a")]
		public string AAA;

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = VisibilitySuppressionJustification)]
		[CLSCompliant(false)]
		[OptionArg(Name = "BBB", ShortNames = new string[] { "b", "a" })]
		public string BBB;

		/// <summary></summary>
		public InvalidCommandLineArgs4(string[] args)
			: base(args, false, true, false)
		{
		}
	}

	/// <summary></summary>
	[SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:FieldsMustBePrivate", Justification = "Command line arguments based on 'MKY.CommandLine.ArgsHandler' must be public.")]
	public class InvalidCommandLineArgs5 : ArgsHandler
	{
		private const string VisibilitySuppressionJustification = "Command line arguments based on 'MKY.CommandLine.ArgsHandler' must be public.";

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = VisibilitySuppressionJustification)]
		[OptionArg(Name = "AAA", ShortName = "a")]
		public string AAA;

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = VisibilitySuppressionJustification)]
		[CLSCompliant(false)]
		[OptionArg(Name = "BBB", ShortNames = new string[] { "a", "b" })]
		public string BBB;

		/// <summary></summary>
		public InvalidCommandLineArgs5(string[] args)
			: base(args, false, true, false)
		{
		}
	}

	/// <summary></summary>
	[SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:FieldsMustBePrivate", Justification = "Command line arguments based on 'MKY.CommandLine.ArgsHandler' must be public.")]
	public class InvalidCommandLineArgs6 : ArgsHandler
	{
		private const string VisibilitySuppressionJustification = "Command line arguments based on 'MKY.CommandLine.ArgsHandler' must be public.";

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = VisibilitySuppressionJustification)]
		[OptionArg(Name = "AAA", ShortName = "a")]
		public string AAA;

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = VisibilitySuppressionJustification)]
		[CLSCompliant(false)]
		[OptionArg(Names = new string[] { "BBB", "AAA" }, ShortName = "b")]
		public string BBB;

		/// <summary></summary>
		public InvalidCommandLineArgs6(string[] args)
			: base(args, false, true, false)
		{
		}
	}

	/// <summary></summary>
	[SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:FieldsMustBePrivate", Justification = "Command line arguments based on 'MKY.CommandLine.ArgsHandler' must be public.")]
	public class InvalidCommandLineArgs7 : ArgsHandler
	{
		private const string VisibilitySuppressionJustification = "Command line arguments based on 'MKY.CommandLine.ArgsHandler' must be public.";

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = VisibilitySuppressionJustification)]
		[OptionArg(Name = "AAA", ShortName = "a")]
		public string AAA;

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = VisibilitySuppressionJustification)]
		[CLSCompliant(false)]
		[OptionArg(Names = new string[] { "AAA", "BBB" }, ShortName = "b")]
		public string BBB;

		/// <summary></summary>
		public InvalidCommandLineArgs7(string[] args)
			: base(args, false, true, false)
		{
		}
	}

	/// <summary></summary>
	[SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:FieldsMustBePrivate", Justification = "Command line arguments based on 'MKY.CommandLine.ArgsHandler' must be public.")]
	public class InvalidCommandLineArgs8 : ArgsHandler
	{
		private const string VisibilitySuppressionJustification = "Command line arguments based on 'MKY.CommandLine.ArgsHandler' must be public.";

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = VisibilitySuppressionJustification)]
		[OptionArg(Name = "AAA", ShortName = "a")]
		public string AAA;

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = VisibilitySuppressionJustification)]
		[CLSCompliant(false)]
		[OptionArg(Names = new string[] { "BBB", "AAA" }, ShortNames = new string[] { "b", "a" })]
		public string BBB;

		/// <summary></summary>
		public InvalidCommandLineArgs8(string[] args)
			: base(args, false, true, false)
		{
		}
	}

	/// <summary></summary>
	[SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:FieldsMustBePrivate", Justification = "Command line arguments based on 'MKY.CommandLine.ArgsHandler' must be public.")]
	public class InvalidCommandLineArgs9 : ArgsHandler
	{
		private const string VisibilitySuppressionJustification = "Command line arguments based on 'MKY.CommandLine.ArgsHandler' must be public.";

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = VisibilitySuppressionJustification)]
		[OptionArg(Name = "AAA", ShortName = "a")]
		public string AAA;

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = VisibilitySuppressionJustification)]
		[CLSCompliant(false)]
		[OptionArg(Names = new string[] { "AAA", "BBB" }, ShortNames = new string[] { "a", "b" })]
		public string BBB;

		/// <summary></summary>
		public InvalidCommandLineArgs9(string[] args)
			: base(args, false, true, false)
		{
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
