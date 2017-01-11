﻿//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at https://sourceforge.net/projects/y-a-terminal/.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Revision$
// $Date$
// $Author$
// ------------------------------------------------------------------------------------------------
// YAT 2.0 Gamma 2'' Version 1.99.52
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

namespace YAT.Model.Types
{
	/// <summary></summary>
	public class PredefinedCommandEventArgs : EventArgs
	{
		private int page;
		private int command;

		/// <summary></summary>
		public PredefinedCommandEventArgs(int command)
			: this(1, command)
		{
		}

		/// <summary></summary>
		public PredefinedCommandEventArgs(int page, int command)
		{
			this.page = page;
			this.command = command;
		}

		/// <summary></summary>
		public int Page
		{
			get { return (this.page); }
		}

		/// <summary></summary>
		public int Command
		{
			get { return (this.command); }
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
