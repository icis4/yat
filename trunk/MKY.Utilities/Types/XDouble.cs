//==================================================================================================
// URL       : $URL$
// Author    : $Author$
// Date      : $Date$
// Revision  : $Rev$
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// ------------------------------------------------------------------------------------------------
// Copyright � 2003-2004 HSR Hochschule f�r Technik Rapperswil.
// Copyright � 2003-2009 Matthias Kl�y.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Collections.Generic;
using System.Text;

namespace MKY.Utilities.Types
{
	/// <summary>
	/// Double utility methods.
	/// </summary>
	public static class XDouble
	{
		/// <summary>
		/// Limits "value" to the boundaries specified.
		/// </summary>
		public static double LimitToBounds(double value, double lower, double upper)
		{
			if (value < lower)
				return (lower);
			if (value > upper)
				return (upper);
			return (value);
		}
	}
}

//==================================================================================================
// End of $URL$
//==================================================================================================
