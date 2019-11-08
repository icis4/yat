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
// YAT Version 2.1.1 Development
// ------------------------------------------------------------------------------------------------
// See release notes for product version details.
// See SVN change log for file revision details.
// Author(s): Matthias Klaey
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2019 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

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
	#if !(WITH_SCRIPTING)
		Success,
		CommandLineError,
		ApplicationStartError,
		ApplicationStartCancel,
		ApplicationRunError,
		ApplicationExitError,
		UnhandledException
	#else
		// Positive values are reserved for the script result!
		Success                  =  0,
		CommandLineError         = -1,
		ApplicationStartError    = -2,
		ApplicationStartCancel   = -3,
		ApplicationRunError      = -4,
		ApplicationExitError     = -5,
		UnhandledException       = -6,
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

	#pragma warning restore 1591
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
