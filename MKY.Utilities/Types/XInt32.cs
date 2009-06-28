//==================================================================================================
// $URL$
// $Author$
// $Date$
// $Revision$
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2009 Matthias Kläy.
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
	/// Int32/int utility methods.
	/// </summary>
	/// <remarks>
	/// Possible extensions:
	/// - ParseInside: get integer values inside strings (e.g. "COM5 (Device123B)" returns {5;123})
	/// </remarks>
	public class XInt32
	{
		/// <summary>
		/// Returns the lesser of the two values.
		/// </summary>
		public static int Min(int value1, int value2)
		{
			if (value1 <= value2)
				return (value1);
			else
				return (value2);
		}

		/// <summary>
		/// Returns the larger of the two values.
		/// </summary>
		public static int Max(int value1, int value2)
		{
			if (value1 >= value2)
				return (value1);
			else
				return (value2);
		}

		/// <summary>
		/// Limits "value" to the boundaries specified.
		/// </summary>
		public static int LimitToBounds(int value, int lower, int upper)
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
// End of
// $URL$
//==================================================================================================
