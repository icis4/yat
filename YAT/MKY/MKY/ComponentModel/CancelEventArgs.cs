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

using System.ComponentModel;

namespace MKY.ComponentModel
{
	/// <summary>
	/// Cancel event args with a value.
	/// </summary>
	public class CancelEventArgs<T> : CancelEventArgs
	{
		/// <summary></summary>
		public T Value { get; }

		/// <summary></summary>
		public CancelEventArgs(T value)
		{
			Value = value;
		}

		/// <summary></summary>
		public CancelEventArgs(T value, bool cancel)
			: base(cancel)
		{
			Value = value;
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
