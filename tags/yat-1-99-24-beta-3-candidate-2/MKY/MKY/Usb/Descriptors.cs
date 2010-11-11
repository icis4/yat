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
// See SVN change log for revision details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2010 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

#region Using
//==================================================================================================
// Using
//==================================================================================================

using System;
using System.Collections.Generic;
using System.Globalization;

#endregion

namespace MKY.Usb
{
	/// <summary>
	/// Encapsulates USB items.
	/// </summary>
	public static class Descriptors
	{
		#region Constants
		//==========================================================================================
		// Constants
		//==========================================================================================

		/// <summary>
		/// Maximum of 126 characters in UCS-2 format.
		/// </summary>
		public const int MaximumStringDescriptorCharLength = 126;

		/// <summary>
		/// 2 x 126 characters + 2 x '\0' results in 254.
		/// </summary>
		public const int MaximumStringDescriptorByteLength = 254;

		#endregion

		#region Static Methods
		//==========================================================================================
		// Static Methods
		//==========================================================================================

		/// <summary>
		/// Retrieves available culture infos from language string.
		/// </summary>
		public static CultureInfo[] GetCultureInfoFromLanguageString(string languageString)
		{
			List<CultureInfo> l = new List<CultureInfo>();
			foreach (char c in languageString)
			{
				int culture = (int)c;
				CultureInfo ci = null;
				try
				{
					ci = CultureInfo.GetCultureInfo(culture);
				}
				catch (ArgumentNullException)
				{
				}
				catch (ArgumentException)
				{
				}

				if (ci != null)
					l.Add(ci);
			}
			return (l.ToArray());
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
