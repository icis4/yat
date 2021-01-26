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
// MKY Version 1.0.29
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

using System;

namespace MKY.Data
{
	/// <summary></summary>
	public class DataEventArgs : EventArgs
	{
		/// <summary></summary>
		public DataItem Source { get; }

		/// <summary></summary>
		public DataEventArgs Inner { get; }

		/// <summary></summary>
		public DataEventArgs(DataItem source)
		{
			Source = source;
		}

		/// <summary></summary>
		public DataEventArgs(DataItem source, DataEventArgs inner)
		{
			Source = source;
			Inner = inner;
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
