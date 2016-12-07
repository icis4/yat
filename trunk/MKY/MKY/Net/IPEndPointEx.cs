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

using System.Diagnostics.CodeAnalysis;

namespace MKY.Net
{
	/// <summary>
	/// <see cref="System.Net.IPEndPoint"/> utility methods.
	/// </summary>
	[SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix", Justification = "'Ex' emphasizes that it's an extension to an existing class and not a replacement as '2' would emphasize.")]
	public static class IPEndPointEx
	{
		/// <summary>
		/// Determines whether the specified <paramref name="port"/> has a valid value.
		/// </summary>
		/// <param name="port">Port to evaluate.</param>
		/// <returns><c>true</c> if value is valid, <c>false</c> otherwise.</returns>
		public static bool IsValidPort(int port)
		{
			return (Int32Ex.IsWithin(port, System.Net.IPEndPoint.MinPort, System.Net.IPEndPoint.MaxPort));
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
