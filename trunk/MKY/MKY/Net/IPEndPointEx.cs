//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at https://sourceforge.net/projects/y-a-terminal/.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Revision$
// $Date$
// $Author$
// ------------------------------------------------------------------------------------------------
// MKY Version 1.0.30
// ------------------------------------------------------------------------------------------------
// See release notes for product version details.
// See SVN change log for file revision details.
// Author(s): Matthias Klaey
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2021 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System.Diagnostics.CodeAnalysis;
using System.Net;

namespace MKY.Net
{
	/// <summary>
	/// <see cref="IPEndPoint"/> utility methods.
	/// </summary>
	[SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix", Justification = "'Ex' emphasizes that it's an extension to an existing class and not a replacement as '2' would emphasize.")]
	public static class IPEndPointEx
	{
		/// <summary>
		/// An empty IP end point, i.e. address 0.0.0.0 and port 0.
		/// </summary>
		/// <remarks>
		/// Must be implemented as property (instead of a readonly) since <see cref="IPEndPoint"/>
		/// is a mutable reference type. Defining a readonly would correctly result in FxCop
		/// message CA2104 "DoNotDeclareReadOnlyMutableReferenceTypes" (Microsoft.Security).
		/// </remarks>
		public static IPEndPoint Empty
		{
			get { return (new IPEndPoint(IPAddress.Any, 0)); }
		}

		/// <summary>
		/// Determines whether the specified <paramref name="port"/> has a valid value.
		/// </summary>
		/// <param name="port">Port to evaluate.</param>
		/// <returns><c>true</c> if value is valid; otherwise, <c>false</c>.</returns>
		public static bool IsValidPort(int port)
		{
			return (Int32Ex.IsWithin(port, IPEndPoint.MinPort, IPEndPoint.MaxPort));
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
