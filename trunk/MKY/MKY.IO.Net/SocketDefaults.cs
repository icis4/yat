//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at http://sourceforge.net/projects/y-a-terminal.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL: https://y-a-terminal.svn.sourceforge.net/svnroot/y-a-terminal/trunk/MKY/MKY.IO.Serial/Socket/SocketDefaults.cs $
// $Author: maettu_this $
// $Date: 2011-04-23 01:49:13 +0200 (Sa, 23 Apr 2011) $
// $Revision: 407 $
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2011 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;

namespace MKY.IO.Net
{
	/// <summary></summary>
	public static class SocketDefaults
	{
		/// <summary></summary>
		public const int FrameSize = 1024;

		/// <summary></summary>
		public const int SocketBufferSize = 2 * FrameSize;

		/// <summary></summary>
		public const int MessageBufferSize = 16 * FrameSize;
	}
}

//==================================================================================================
// End of
// $URL: https://y-a-terminal.svn.sourceforge.net/svnroot/y-a-terminal/trunk/MKY/MKY.IO.Serial/Socket/SocketDefaults.cs $
//==================================================================================================
