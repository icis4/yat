//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at http://sourceforge.net/projects/y-a-terminal/.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Author$
// $Date$
// $Revision$
// ------------------------------------------------------------------------------------------------
// MKY Development Version 1.0.10
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// See release notes for product version details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2010-2015 Matthias Kläy.
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
using System.Diagnostics.CodeAnalysis;
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
		public const int MaxStringDescriptorCharLength = 126;

		/// <summary>
		/// 2 x 126 characters + 2 x '\0' results in 254.
		/// </summary>
		public const int MaxStringDescriptorByteLength = 254;

		#endregion

		#region Static Methods
		//==========================================================================================
		// Static Methods
		//==========================================================================================

		/// <summary>
		/// Retrieves available culture information from language string.
		/// </summary>
		[SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "string", Justification = "The parameter shall clearly state what it represents.")]
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
