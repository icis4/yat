//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at https://sourceforge.net/projects/y-a-terminal/.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Author$
// $Date$
// $Revision$
// ------------------------------------------------------------------------------------------------
// YAT 2.0 Gamma 2 Development Version 1.99.35
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// See release notes for product version details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2016 Matthias Kläy.
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
using System.IO;
using System.Text;

using MKY;
using MKY.CommandLine;
using MKY.Settings;

using YAT.Settings.Application;
using YAT.Settings.Terminal;
using YAT.Settings.Workspace;

#endregion

namespace YAT.Model
{
	/// <summary></summary>
	[SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:FieldsMustBePrivate", Justification = "Start arguments are implemented as public just as command line arguments are.")]
	public class MainStartArgs
	{
		private const string VisibilitySuppressionJustification = "Start arguments are implemented as public just as command line arguments are.";

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = VisibilitySuppressionJustification)]
		public DocumentSettingsHandler<WorkspaceSettingsRoot> WorkspaceSettingsHandler;

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = VisibilitySuppressionJustification)]
		public DocumentSettingsHandler<TerminalSettingsRoot> TerminalSettingsHandler;

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = VisibilitySuppressionJustification)]
		public bool TileHorizontal;

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = VisibilitySuppressionJustification)]
		public bool TileVertical;

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = VisibilitySuppressionJustification)]
		public int RequestedDynamicTerminalIndex = Indices.DefaultDynamicIndex;

		/// <remarks>Using term 'Transmit' to indicate potential 'intelligence' to send + receive/verify the data.</remarks>
		[SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = VisibilitySuppressionJustification)]
		public string RequestedTransmitText;

		/// <remarks>Using term 'Transmit' to indicate potential 'intelligence' to send + receive/verify the data.</remarks>
		[SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = VisibilitySuppressionJustification)]
		public string RequestedTransmitFilePath;

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = VisibilitySuppressionJustification)]
		public bool PerformOperationOnRequestedTerminal;

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = VisibilitySuppressionJustification)]
		public int OperationDelay;

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = VisibilitySuppressionJustification)]
		public int ExitDelay;

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = VisibilitySuppressionJustification)]
		public bool ShowNewTerminalDialog;

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = VisibilitySuppressionJustification)]
		public bool KeepOpen;

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = VisibilitySuppressionJustification)]
		public bool KeepOpenOnError;

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = VisibilitySuppressionJustification)]
		public bool NonInteractive;

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = VisibilitySuppressionJustification)]
		public string ErrorMessage;

		/// <summary></summary>
		public WorkspaceStartArgs ToWorkspaceStartArgs()
		{
			WorkspaceStartArgs args = new WorkspaceStartArgs();

			args.KeepOpen        = this.KeepOpen;
			args.KeepOpenOnError = this.KeepOpenOnError;
			args.NonInteractive  = this.NonInteractive;

			return (args);
		}

		/// <summary></summary>
		public TerminalStartArgs ToTerminalStartArgs()
		{
			TerminalStartArgs args = new TerminalStartArgs();

			args.KeepOpen        = this.KeepOpen;
			args.KeepOpenOnError = this.KeepOpenOnError;
			args.NonInteractive  = this.NonInteractive;

			return (args);
		}

		/// <summary>
		/// Returns whether user or other interaction shall be permitted.
		/// </summary>
		public bool Interactive
		{
			get { return (!(NonInteractive)); }
		}
	}

	/// <summary></summary>
	[SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:FieldsMustBePrivate", Justification = "Start arguments are implemented as public just as command line arguments are.")]
	public class WorkspaceStartArgs
	{
		private const string VisibilitySuppressionJustification = "Start arguments are implemented as public just as command line arguments are.";

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = VisibilitySuppressionJustification)]
		public bool KeepOpen;

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = VisibilitySuppressionJustification)]
		public bool KeepOpenOnError;

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = VisibilitySuppressionJustification)]
		public bool NonInteractive;

		/// <summary></summary>
		public TerminalStartArgs ToTerminalStartArgs()
		{
			TerminalStartArgs args = new TerminalStartArgs();

			args.KeepOpen        = this.KeepOpen;
			args.KeepOpenOnError = this.KeepOpenOnError;
			args.NonInteractive  = this.NonInteractive;

			return (args);
		}

		/// <summary>
		/// Returns whether user or other interaction shall be permitted.
		/// </summary>
		public bool Interactive
		{
			get { return (!(NonInteractive)); }
		}
	}

	/// <summary></summary>
	[SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:FieldsMustBePrivate", Justification = "Start arguments are implemented as public just as command line arguments are.")]
	public class TerminalStartArgs
	{
		private const string VisibilitySuppressionJustification = "Start arguments are implemented as public just as command line arguments are.";

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = VisibilitySuppressionJustification)]
		public bool KeepOpen;

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = VisibilitySuppressionJustification)]
		public bool KeepOpenOnError;

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = VisibilitySuppressionJustification)]
		public bool NonInteractive;

		/// <summary>
		/// Returns whether user or other interaction shall be permitted.
		/// </summary>
		public bool Interactive
		{
			get { return (!(NonInteractive)); }
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
