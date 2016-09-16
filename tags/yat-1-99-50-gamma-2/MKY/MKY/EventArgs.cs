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
// YAT 2.0 Gamma 2 Version 1.99.50
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

namespace MKY
{
	/// <summary>
	/// Generic event args with a (simple) type.
	/// </summary>
	/// <typeparam name="T">(Simple) type of the event args.</typeparam>
	public class EventArgs<T> : EventArgs
	{
		private T value;

		/// <summary></summary>
		public EventArgs(T value)
		{
			this.value = value;
		}

		/// <summary></summary>
		public T Value
		{
			get { return (this.value); }
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
