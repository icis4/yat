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

namespace YAT.Model.Types
{
	/// <summary>
	/// Specifies the mode how the terminal monitors are updated.
	/// </summary>
	public enum MonitorUpdateMode
	{
		/// <summary>
		/// Monitors are updated on each added element, i.e. providing immediate feedback.
		/// </summary>
		/// <remarks>
		/// This mode is the default mode.
		/// </remarks>
		Element,

		/// <summary>
		/// Monitors are updated on completed lines, i.e. retaining elements until a line is completed.
		/// </summary>
		/// <remarks>
		/// This mode is chosen when filtering or supression is active, as filtering or supression
		/// can only be evaluated on complete lines. (With the exception for cases where the trigger
		/// is already contained in the first chunk of a line. However, it is considered good enough
		/// to always behave the same, i.e. don't optimize for such cases.
		/// </remarks>
		Line
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
