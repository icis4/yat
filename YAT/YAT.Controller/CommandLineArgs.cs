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
// YAT 2.0 Beta 4 Candidate 1 Development Version 1.99.27
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

using MKY.CommandLine;

#endregion

namespace YAT.Controller
{
	/// <summary></summary>
	public class CommandLineArgs : Model.CommandLineArgs
	{
		private const string SuppressionJustification = "This is a simple container for command line args.";

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = SuppressionJustification)]
		[OptionArg(Name = "NoLogo", ShortName = "nl", Description = "Do not display title and copyright.")]
		public bool NoLogo;

		/// <summary></summary>
		[CLSCompliant(false)]
		[SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = SuppressionJustification)]
		[OptionArg(Names = new string[] { "Help", "HelpText" }, ShortNames = new string[] { "h", "?" }, Description = "Display this help text.")]
		public bool HelpIsRequested;

		/// <summary></summary>
		public CommandLineArgs(string[] args)
			: base(args)
		{
		}

		/// <summary>
		/// Gets a value indicating whether [show logo].
		/// </summary>
		public bool ShowLogo
		{
			get { return (!(NoLogo)); }
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
