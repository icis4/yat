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
// YAT Version 2.3.90 Development
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

// This code is intentionally placed into the YAT.View.Controls namespace even though the file is
// located in YAT.View.Controls.Types for same location as parent control.
namespace YAT.View.Controls
{
	/// <summary>
	/// The result of a find operation.
	/// </summary>
	public enum FindDirection
	{
		/// <summary>Find has not been triggered yet, or find has been reset.</summary>
		Undetermined,

		/// <summary>Find next, i.e. forward.</summary>
		Forward,

		/// <summary>Find previous, i.e. backward.</summary>
		Backward
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
