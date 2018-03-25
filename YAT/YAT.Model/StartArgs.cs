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
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2018 Matthias Kläy.
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
	public struct Override : IEquatable<Override>
	{
		/// <summary></summary>
		public bool StartTerminal { get; set; }

		/// <summary></summary>
		public bool KeepTerminalStopped { get; set; }

		/// <summary></summary>
		public bool LogOn { get; set; }

		/// <summary></summary>
		public bool KeepLogOff { get; set; }

		#region Object Members
		//==========================================================================================
		// Object Members
		//==========================================================================================

		/// <summary>
		/// Converts the value of this instance to its equivalent string representation.
		/// </summary>
		/// <remarks>
		/// Use properties instead of fields. This ensures that 'intelligent' properties,
		/// i.e. properties with some logic, are also properly handled.
		/// </remarks>
		public override string ToString()
		{
			return
			(
				StartTerminal       + ", " +
				KeepTerminalStopped + ", " +
				LogOn               + ", " +
				KeepLogOff
			);
		}

		/// <summary>
		/// Serves as a hash function for a particular type.
		/// </summary>
		/// <remarks>
		/// Use properties instead of fields to calculate hash code. This ensures that 'intelligent'
		/// properties, i.e. properties with some logic, are also properly handled.
		/// </remarks>
		public override int GetHashCode()
		{
			unchecked
			{
				int hashCode;

				hashCode =                    StartTerminal      .GetHashCode();
				hashCode = (hashCode * 397) ^ KeepTerminalStopped.GetHashCode();
				hashCode = (hashCode * 397) ^ LogOn              .GetHashCode();
				hashCode = (hashCode * 397) ^ KeepLogOff         .GetHashCode();

				return (hashCode);
			}
		}

		/// <summary>
		/// Determines whether this instance and the specified object have value equality.
		/// </summary>
		public override bool Equals(object obj)
		{
			if (obj is Override)
				return (Equals((Override)obj));
			else
				return (false);
		}

		/// <summary>
		/// Determines whether this instance and the specified object have value equality.
		/// </summary>
		/// <remarks>
		/// Use properties instead of fields to determine equality. This ensures that 'intelligent'
		/// properties, i.e. properties with some logic, are also properly handled.
		/// </remarks>
		public bool Equals(Override other)
		{
			return
			(
				StartTerminal      .Equals(other.StartTerminal)       &&
				KeepTerminalStopped.Equals(other.KeepTerminalStopped) &&
				LogOn              .Equals(other.LogOn)               &&
				KeepLogOff         .Equals(other.KeepLogOff)
			);
		}

		/// <summary>
		/// Determines whether the two specified objects have value equality.
		/// </summary>
		public static bool operator ==(Override lhs, Override rhs)
		{
			return (lhs.Equals(rhs));
		}

		/// <summary>
		/// Determines whether the two specified objects have value inequality.
		/// </summary>
		public static bool operator !=(Override lhs, Override rhs)
		{
			return (!(lhs == rhs));
		}

		#endregion
	}

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
		public Override Override;

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = VisibilitySuppressionJustification)]
		public bool TileHorizontal;

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = VisibilitySuppressionJustification)]
		public bool TileVertical;

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = VisibilitySuppressionJustification)]
		public int RequestedDynamicTerminalId = TerminalIds.ActiveDynamicId;

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
			var args = new WorkspaceStartArgs();

			args.Override = this.Override;

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
		public Override Override;

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
			var args = new TerminalStartArgs();

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
