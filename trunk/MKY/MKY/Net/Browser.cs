﻿//==================================================================================================
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
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2010 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;

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
		public static void BrowseUri(string uri)
		{
			System.Diagnostics.Process.Start(uri);
		}

		/// <summary>
		/// Opens the system default browser and browses uri.
		/// </summary>
		/// <param name="uri">URI to browse.</param>
		public static void BrowseUri(Uri uri)
		{
			BrowseUri(uri.AbsoluteUri);
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
