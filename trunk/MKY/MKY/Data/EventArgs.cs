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
// MKY Version 1.0.17
// ------------------------------------------------------------------------------------------------
// See release notes for product version details.
// See SVN change log for file revision details.
// Author(s): Matthias Klaey
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2017 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;

namespace MKY.Data
{
	/// <summary></summary>
	public class DataEventArgs : EventArgs
	{
		private DataItem source;
		private DataEventArgs inner;

		/// <summary></summary>
		public DataEventArgs(DataItem source)
		{
			this.source = source;
		}

		/// <summary></summary>
		public DataEventArgs(DataItem source, DataEventArgs inner)
		{
			this.source = source;
			this.inner = inner;
		}

		/// <summary></summary>
		public DataItem Source
		{
			get { return (this.source); }
		}

		/// <summary></summary>
		public DataEventArgs Inner
		{
			get { return (this.inner); }
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
