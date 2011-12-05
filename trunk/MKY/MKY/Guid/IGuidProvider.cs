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
// MKY Development Version 1.0.8
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// See release notes for product version details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2011 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;

// This code is intentionally placed into the MKY namespace even though the file is located in
// MKY.Guid for consistency with the Sytem.Guid class.
namespace MKY
{
	/// <summary>
	/// Interface that can be implemented by types providing a <see cref="Guid"/>.
	/// </summary>
	public interface IGuidProvider
	{
		/// <summary>
		/// Returns the <see cref="Guid"/> of the providing object.
		/// </summary>
		Guid Guid { get; }
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
