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
// YAT Version 2.2.0 Development
// ------------------------------------------------------------------------------------------------
// See release notes for product version details.
// See SVN change log for file revision details.
// Author(s): Matthias Klaey
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2020 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;

namespace YAT.Model.Types
{
	/// <summary></summary>
	public class PredefinedCommandEventArgs : EventArgs
	{
		/// <summary></summary>
		public int PageId { get; }

		/// <summary></summary>
		public int CommandId { get; }

		/// <summary></summary>
		public PredefinedCommandEventArgs(int commandId)
			: this(PredefinedCommandPageCollection.NoPageId, commandId)
		{
		}

		/// <summary></summary>
		public PredefinedCommandEventArgs(int pageId, int commandId)
		{
			PageId    = pageId;
			CommandId = commandId;
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
