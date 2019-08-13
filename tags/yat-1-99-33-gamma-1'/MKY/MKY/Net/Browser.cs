﻿//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at http://sourceforge.net/projects/y-a-terminal/.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Author$
// $Date$
// $Revision$
// ------------------------------------------------------------------------------------------------
// MKY Version 1.0.12
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
using System.Diagnostics.CodeAnalysis;

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
		[SuppressMessage("Microsoft.Design", "CA1057:StringUriOverloadsCallSystemUriOverloads", Justification = "Quite funny suggestion by FxCop...")]
		public static void BrowseUri(string uri)
		{
			Process.Start(uri);
		}

		/// <summary>
		/// Opens the system default browser and browses uri.
		/// </summary>
		/// <param name="uri">URI to browse.</param>
		[SuppressMessage("Microsoft.Usage", "CA2234:PassSystemUriObjectsInsteadOfStrings", Justification = "Quite funny suggestion by FxCop...")]
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