//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at http://sourceforge.net/projects/y-a-terminal.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
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
using System.Collections.Generic;
using System.Text;

namespace MKY.Guid
{
	/// <summary>
	/// Interface that can be implemented by types providing a <see cref="Guid"/>.
	/// </summary>
	public interface IGuidProvider
	{
		/// <summary>
		/// Returns the <see cref="Guid"/> of the providing object.
		/// </summary>
		System.Guid Guid { get; }
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
