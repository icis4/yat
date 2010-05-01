//==================================================================================================
// $URL$
// $Author$
// $Date$
// $Revision$
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2010 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;

namespace MKY.Utilities
{
	/// <summary></summary>
	public static class Unused
	{
		/// <summary>
		/// Utility method that can be applied to unused objects to prevent compiler warnings.
		/// </summary>
		public static void PreventCompilerWarning(object obj)
		{
			if ((bool)obj)
				return;

			// else return too...
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
