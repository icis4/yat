//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at http://sourceforge.net/projects/y-a-terminal.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Author$
// $Date$
// $Revision$
// ------------------------------------------------------------------------------------------------
// MKY Development Version 1.0.6
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// See release notes for product version details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2011 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Collections.Generic;
using System.Text;

// This code is intentionally placed into the MKY namespace even though the file is located in
// MKY.Types for consistency with the Sytem namespace.
namespace MKY
{
	/// <summary>
	/// Bool/bool utility methods.
	/// </summary>
	public class BoolEx
	{
		/// <summary>
		/// Toggles a boolean value.
		/// </summary>
		public static void Toggle(ref bool value)
		{
			value = (!value);
		}

		/// <summary>
		/// Sets the value if it isn't set yet. Return whether the value has been set.
		/// </summary>
		/// <remarks>
		/// Calling this function is more obvious than implementing the logic directly.
		/// Thus, this function has been created to improve readability of the code and
		/// reduce potential errors.
		/// </remarks>
		public static bool SetIfCleared(ref bool value)
		{
			if (!value)
			{
				value = true;
				return (true);
			}
			else
			{
				return (false);
			}
		}

		/// <summary>
		/// Clears the value if it is set. Return whether the value has been cleared.
		/// </summary>
		/// <remarks>
		/// Calling this function is more obvious than implementing the logic directly.
		/// Thus, this function has been created to improve readability of the code and
		/// reduce potential errors.
		/// </remarks>
		public static bool ClearIfSet(ref bool value)
		{
			if (value)
			{
				value = false;
				return (true);
			}
			else
			{
				return (false);
			}
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
