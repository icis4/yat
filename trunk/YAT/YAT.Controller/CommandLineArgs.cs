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
using System.Text;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

using MKY.CommandLine;

using YAT.Utilities;

#endregion

namespace YAT.Controller
{
	/// <summary></summary>
	public class CommandLineArgs : Model.CommandLineArgs
	{
		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = "This is a simple container for command line args.")]
		[OptionArg(Name = "NoLogo", ShortName = "nl", Description = "Do not display title and copyright.")]
		[CLSCompliant(false)]
		public bool NoLogo;

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = "This is a simple container for command line args.")]
		[OptionArg(Name = "Help", ShortNames = new string[] { "h", "?" }, Description = "Display this help text.")]
		[CLSCompliant(false)]
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

		/// <summary>
		/// Gets the help text.
		/// </summary>
		public override string GetHelpText()
		{
			StringBuilder helpText = new StringBuilder();

			helpText.AppendLine(                   "Usage:");
			helpText.AppendLine();
			helpText.AppendLine(MinorIndentSpace +   "YAT[.exe] [<Workspace>.yaw|<Terminal>.yat] [<Options>]");
			helpText.AppendLine();
			helpText.AppendLine();
			helpText.AppendLine(                   "Usage examples:");
			helpText.AppendLine();
			helpText.AppendLine(MinorIndentSpace +   "YAT MyWorkspace.yaw");
			helpText.AppendLine(MajorIndentSpace +           "Start YAT and open given workspace.");
			helpText.AppendLine();
			helpText.AppendLine(MinorIndentSpace +   "YAT MyTerminal.yat");
			helpText.AppendLine(MajorIndentSpace +           "Start YAT and open given terminal.");
			helpText.AppendLine();
			helpText.AppendLine(MinorIndentSpace +   "YAT /r");
			helpText.AppendLine(MajorIndentSpace +           "Start YAT and open most recent file.");
			helpText.AppendLine();

			helpText.Append(base.GetHelpText());

			return (helpText.ToString());
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
