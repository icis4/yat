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
// YAT Version 2.2.0 Development
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

using System.Diagnostics.CodeAnalysis;

namespace YAT.Model
{
	// Disable warning 1591 "Missing XML comment for publicly visible type or member" to avoid
	// warnings for each undocumented member below. Documenting each member makes little sense
	// since they pretty much tell their purpose and documentation tags between the members
	// makes the code less readable.
	#pragma warning disable 1591

	/// <summary>
	/// Enumeration of all the main result return codes.
	/// </summary>
	public enum MainResult
	{
	#if (WITH_SCRIPTING)
		// Positive values are reserved for script result!
	#endif
		Success                  =  0,
		CommandLineError         = -1,
		ApplicationStartCancel   = -2,
		ApplicationStartError    = -3,
		ApplicationRunError      = -4,
		ApplicationExitError     = -5,
		UnhandledException       = -6,
	#if (WITH_SCRIPTING)
		ScriptInvalidContent     = MT.Albatros.Core.RunResult.ScriptInvalidContent,
		ScriptStopOnError        = MT.Albatros.Core.RunResult.ScriptStopOnError,
		ScriptExit               = MT.Albatros.Core.RunResult.ScriptExit,
		ScriptUserBreak          = MT.Albatros.Core.RunResult.ScriptUserBreak,
		ScriptUnhandledException = MT.Albatros.Core.RunResult.ScriptUnhandledException,
	////ScriptInvalidReturnValue = MT.Albatros.Core.RunResult.ScriptInvalidReturnValue, \fixme (2017-02-14 / MKY) legacy...
		ScriptThreadAbort        = MT.Albatros.Core.RunResult.ThreadAbort,
		ScriptRemotingException  = MT.Albatros.Core.RunResult.RemotingException,
		ScriptInvalidOperation   = MT.Albatros.Core.RunResult.InvalidOperation
	#endif
	}

	/// <summary>
	/// Enumeration of the I/O availability check results.
	/// </summary>
	public enum CheckResult
	{
		[SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "OK", Justification = "Same spelling as 'DialogResult.OK'.")]
		OK,
		Cancel,
		Ignore
	}

	/// <summary>
	/// Enumeration of the applications, exit mode.
	/// </summary>
	public enum ExitMode
	{
		None,
		Manual,
		Auto
	}

	#pragma warning restore 1591
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
