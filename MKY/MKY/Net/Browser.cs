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
// MKY Development Version 1.0.14
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// See release notes for product version details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2015 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Diagnostics;

namespace MKY.Net
{
	/// <summary>
	/// Browser utility methods.
	/// </summary>
	public static class Browser
	{
		/// <summary>
		/// Opens the system default browser and browses uri.
		/// </summary>
		/// <param name="uri">URI to browse.</param>
		/// <param name="exception">Exception object, in case of failure.</param>
		/// <returns><c>true</c> if successful, <c>false</c> otherwise.</returns>
		public static bool BrowseUri(string uri, out Exception exception)
		{
			try
			{
				Process.Start(uri);
				exception = null;
				return (true);
			}
			catch (Exception ex)
			{
				exception = ex;
				return (false);
			}
		}

		/// <summary>
		/// Opens the system default browser and browses uri.
		/// </summary>
		/// <param name="uri">URI to browse.</param>
		/// <param name="exception">Exception object, in case of failure.</param>
		/// <returns><c>true</c> if successful, <c>false</c> otherwise.</returns>
		public static bool BrowseUri(Uri uri, out Exception exception)
		{
			return (BrowseUri(uri.AbsoluteUri, out exception));
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
