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
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2021 Matthias Kläy.
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

using YAT.Settings.Model;

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

	/// <remarks>
	/// Using term "launch" rather than "start" for distinction with "start/stop" I/O.
	/// </remarks>
	[SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:FieldsMustBePrivate", Justification = "Launch arguments are implemented as public just as command line arguments are.")]
	public class MainLaunchArgs
	{
		private const string VisibilitySuppressionJustification = "Launch arguments are implemented as public just as command line arguments are.";

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

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = VisibilitySuppressionJustification)]
		public int RequestedFixedTerminalId = TerminalIds.ActiveFixedId;

		/// <remarks>Using term "Transmit" to indicate potential "intelligence" to send + receive/verify the data.</remarks>
		[SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = VisibilitySuppressionJustification)]
		public string RequestedTransmitText;

		/// <remarks>Using term "Transmit" to indicate potential "intelligence" to send + receive/verify the data.</remarks>
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

	#if (WITH_SCRIPTING)

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = VisibilitySuppressionJustification)]
		public string RequestedScriptFilePath;

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = VisibilitySuppressionJustification)]
		public string RequestedScriptLogFilePath;

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = VisibilitySuppressionJustification)]
		public bool AppendTimeStampToScriptLogFileName;

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = VisibilitySuppressionJustification)]
		public string[] RequestedScriptArgs;

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = VisibilitySuppressionJustification)]
		public bool ScriptRunIsRequested;

	#endif // WITH_SCRIPTING

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = VisibilitySuppressionJustification)]
		public bool ShowNewTerminalDialog;

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = VisibilitySuppressionJustification)]
		public bool KeepOpen;

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = VisibilitySuppressionJustification)]
		public bool KeepOpenOnNonSuccess;

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = VisibilitySuppressionJustification)]
		public bool NonInteractive;

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = VisibilitySuppressionJustification)]
		public string MessageOnFailure;

		/// <summary></summary>
		public WorkspaceLaunchArgs ToWorkspaceLaunchArgs()
		{
			var args = new WorkspaceLaunchArgs();

			args.Override = this.Override;

			args.KeepOpen             = this.KeepOpen;
			args.KeepOpenOnNonSuccess = this.KeepOpenOnNonSuccess;
			args.NonInteractive       = this.NonInteractive;

			return (args);
		}

		/// <summary>
		/// Returns whether user or other interaction shall be permitted.
		/// </summary>
		public bool Interactive
		{
			get { return (!(NonInteractive)); }
		}

		/// <summary>
		/// Returns whether the application has been launched to perform an automatic run,
		/// i.e. an automatic operation or a script run.
		/// </summary>
		public bool IsAutoRun
		{
			get
			{
			#if (!WITH_SCRIPTING)
				return (PerformOperationOnRequestedTerminal);
			#else
				return (PerformOperationOnRequestedTerminal || ScriptRunIsRequested);
			#endif
			}
		}
	}

	/// <remarks>
	/// Using term "launch" rather than "start" for distinction with "start/stop" I/O.
	/// </remarks>
	[SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:FieldsMustBePrivate", Justification = "Launch arguments are implemented as public just as command line arguments are.")]
	public class WorkspaceLaunchArgs
	{
		private const string VisibilitySuppressionJustification = "Launch arguments are implemented as public just as command line arguments are.";

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = VisibilitySuppressionJustification)]
		public Override Override;

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = VisibilitySuppressionJustification)]
		public bool KeepOpen;

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = VisibilitySuppressionJustification)]
		public bool KeepOpenOnNonSuccess;

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = VisibilitySuppressionJustification)]
		public bool NonInteractive;

		/// <summary></summary>
		public TerminalLaunchArgs ToTerminalLaunchArgs()
		{
			var args = new TerminalLaunchArgs();

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

	/// <remarks>
	/// Using term "launch" rather than "start" for distinction with "start/stop" I/O.
	/// </remarks>
	[SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:FieldsMustBePrivate", Justification = "Launch arguments are implemented as public just as command line arguments are.")]
	public class TerminalLaunchArgs
	{
		private const string VisibilitySuppressionJustification = "Launch arguments are implemented as public just as command line arguments are.";

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
