//==================================================================================================
// $URL$
// $Author$
// $Date$
// $Revision$
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2010 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// YAT is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Collections.Generic;
using System.Text;

namespace YAT.Controller
{
	/// <summary>
	/// Enummeration of all the main result return codes.
	/// </summary>
	public enum MainResult
	{
		OK = 0,
		CommandLineArgsError = -1,
		ApplicationSettingsError = -2,
		ApplicationStartError = -3,
		ApplicationExitError = -4,
		UnhandledException = -5,
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
