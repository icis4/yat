//==================================================================================================
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
using System.Runtime.InteropServices;

namespace MKY.Utilities.Win32
{
	/// <summary>
	/// Encapsulates general constants of the Win32 API.
	/// </summary>
	public static class Constants
	{
		/// <summary></summary>
		public static readonly IntPtr InvalidHandle = new IntPtr(-1);

		/// <summary></summary>
		public const int WAIT_TIMEOUT = 0x0102;
	
		/// <summary></summary>
		public const int WAIT_OBJECT_0 = 0;
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
