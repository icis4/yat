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
// YAT Version 2.4.1
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

namespace YAT.Application
{
	// Disable warning 1591 "Missing XML comment for publicly visible type or member" to avoid
	// warnings for each undocumented member below. Documenting each member makes little sense
	// since they pretty much tell their purpose and documentation tags between the members
	// makes the code less readable.
	#pragma warning disable 1591

	/// <summary>
	/// Enumeration of all the result return codes.
	/// </summary>
	[SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification = "Intentionally using nested type, instead of replicating the parent's name to 'MainResult'.")]
	public enum MainResult
	{
	#if (WITH_SCRIPTING)
		// Positive values are reserved for script result!
	#endif
		Success                  =  0,
		CommandLineError         = -1,
		SystemEnvironmentError   = -2,
		ApplicationSettingsError = -3,
		ApplicationLaunchCancel  = -4,
		ApplicationLaunchError   = -5,
		ApplicationRunError      = -6,
		ApplicationExitError     = -7,
		UnhandledException       = -8,
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

	#pragma warning restore 1591
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
