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

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;

using MKY;
using MKY.CommandLine;

using YAT.Settings;
using YAT.Settings.Application;

#endregion

namespace YAT.Model
{
	/// <summary></summary>
	public class CommandLineArgs : ArgsHandler
	{
		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = "This is a simple container for command line args.")]
		[ValueArg(Description = "The YAT workspace (.yaw) or terminal (.yat) file to open")]
		public string RequestedFilePath;

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = "This is a simple container for command line args.")]
		[OptionArg(Name = "Recent", ShortName = "r", Description = "Open the most recent file")]
		public bool MostRecentIsRequested;

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = "This is a simple container for command line args.")]
		[OptionArg(Name = "Terminal", ShortName = "t", Description = "Perform any initial operation on the terminal with the given sequential index within the opening workspace")]
		public int RequestedSequentialTerminalIndex;

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = "This is a simple container for command line args.")]
		[OptionArg(Name = "TransmitFile", ShortName = "tf", Description = "Automatically transmit the given file using the terminal specified")]
		public string RequestedTransmitFilePath;

		/// <summary></summary>
		public CommandLineArgs(string[] args)
			: base(args)
		{
		}

		/// <summary>
		/// Processes the command line options and validates them.
		/// </summary>
		protected override bool Validate()
		{
			bool isValid = true;

			// RequestedFilePath:
			if (File.Exists(RequestedFilePath))
			{
				if (!ExtensionSettings.IsWorkspaceFile(Path.GetExtension(RequestedFilePath)) &&
					!ExtensionSettings.IsTerminalFile (Path.GetExtension(RequestedFilePath)))
				{
					RequestedFilePath = null;
					BoolEx.ClearIfSet(ref isValid);
				}
			}
			else
			{
				RequestedFilePath = null;
				BoolEx.ClearIfSet(ref isValid);
			}

			// Recent:
			if (MostRecentIsRequested)
			{
				ApplicationSettings.LocalUser.RecentFiles.FilePaths.ValidateAll();
				bool thereAreRecents = (ApplicationSettings.LocalUser.RecentFiles.FilePaths.Count > 0);
				if (thereAreRecents)
					RequestedFilePath = ApplicationSettings.LocalUser.RecentFiles.FilePaths[0].Item;
			}

			return (isValid);
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
