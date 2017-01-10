//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at https://sourceforge.net/projects/y-a-terminal/.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Author$
// $Date$
// $Revision$
// ------------------------------------------------------------------------------------------------
// MKY Version 1.0.17
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// See release notes for product version details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2016 Matthias Kläy.
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
		/// Tries to browse the given URI with the system's default browser.
		/// </summary>
		/// <param name="uri">URI to browse.</param>
		/// <returns><c>true</c> if successful; otherwise, <c>false</c>.</returns>
		[SuppressMessage("Microsoft.Design", "CA1057:StringUriOverloadsCallSystemUriOverloads", Justification = "Process.Start() requires a string.")]
		[SuppressMessage("Microsoft.Usage", "CA2234:PassSystemUriObjectsInsteadOfStrings", Justification = "Process.Start() requires a string.")]
		public static bool TryBrowseUri(string uri)
		{
			Exception exception;
			return (TryBrowseUri(uri, out exception));
		}

		/// <summary>
		/// Tries to browse the given URI with the system's default browser.
		/// </summary>
		/// <param name="uri">URI to browse.</param>
		/// <param name="exception">Exception object, in case of failure.</param>
		/// <returns><c>true</c> if successful; otherwise, <c>false</c>.</returns>
		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Ensure that operation succeeds in any case.")]
		[SuppressMessage("Microsoft.Design", "CA1057:StringUriOverloadsCallSystemUriOverloads", Justification = "Process.Start() requires a string.")]
		public static bool TryBrowseUri(string uri, out Exception exception)
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
		/// Tries to browse the given URI with the system's default browser.
		/// </summary>
		/// <param name="uri">URI to browse.</param>
		/// <returns><c>true</c> if successful; otherwise, <c>false</c>.</returns>
		public static bool TryBrowseUri(Uri uri)
		{
			Exception exception;
			return (TryBrowseUri(uri, out exception));
		}

		/// <summary>
		/// Tries to browse the given URI with the system's default browser.
		/// </summary>
		/// <param name="uri">URI to browse.</param>
		/// <param name="exception">Exception object, in case of failure.</param>
		/// <returns><c>true</c> if successful; otherwise, <c>false</c>.</returns>
		public static bool TryBrowseUri(Uri uri, out Exception exception)
		{
			return (TryBrowseUri(uri.AbsoluteUri, out exception));
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
