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
// Copyright © 2003-2020 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

// This code is intentionally placed into the YAT.View.Forms namespace even though the file is
// located in YAT.View.Forms.Types for same location as parent control.
namespace YAT.View.Forms
{
	/// <summary>
	/// The state of an Auto[Action|Response] content.
	/// </summary>
	public enum AutoContentState
	{
		/// <summary>Content is valid, or not defined yet.</summary>
		Neutral,

		/// <summary>Content is invalid.</summary>
		Invalid
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
