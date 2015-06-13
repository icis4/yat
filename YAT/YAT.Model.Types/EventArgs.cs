//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at http://sourceforge.net/projects/y-a-terminal/.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Author$
// $Date$
// $Revision$
// ------------------------------------------------------------------------------------------------
// YAT 2.0 Gamma 2 Development Version 1.99.35
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// See release notes for product version details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2015 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// YAT is licensed under the GNU LGPL.
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
