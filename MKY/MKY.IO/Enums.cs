//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at http://sourceforge.net/projects/y-a-terminal/.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL: https://y-a-terminal.svn.sourceforge.net/svnroot/y-a-terminal/trunk/MKY/MKY.IO.Serial/Enums.cs $
// $Author: klaey-1 $
// $Date: 2011/08/24 13:38:45MESZ $
// $Revision: 1.1 $
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2012 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

// Disable warning 1591 "Missing XML comment for publicly visible type or member" to avoid
// warnings for each undocumented member below. Documenting each member makes little sense
// since they pretty much tell their purpose and documentation tags between the members
// makes the code less readable.
#pragma warning disable 1591

using System;
using System.Collections.Generic;
using System.Text;

namespace MKY.IO
{
	/// <summary></summary>
	public enum IORequest
	{
		Open,
		Close,
	}

	/// <summary></summary>
	public enum IOErrorSeverity
	{
		Acceptable,
		Severe,
		Fatal,
	}

	/// <summary></summary>
	public enum IODirection
	{
		Any,
		Input,
		Output,
	}
}

//==================================================================================================
// End of
// $URL: https://y-a-terminal.svn.sourceforge.net/svnroot/y-a-terminal/trunk/MKY/MKY.IO.Serial/Enums.cs $
//==================================================================================================
