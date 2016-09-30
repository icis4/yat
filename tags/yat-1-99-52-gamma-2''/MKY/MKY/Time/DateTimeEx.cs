﻿//==================================================================================================
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

// This code is intentionally placed into the MKY namespace even though the file is located in
// MKY.Times for consistency with the Sytem namespace.
namespace MKY
{
	/// <summary></summary>
	public class DateTimeEventArgs : EventArgs
	{
		private DateTime dateTime;

		/// <summary></summary>
		public DateTimeEventArgs(DateTime dateTime)
		{
			this.dateTime = dateTime;
		}

		/// <summary></summary>
		public DateTime DateTime
		{
			get { return (this.dateTime); }
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
