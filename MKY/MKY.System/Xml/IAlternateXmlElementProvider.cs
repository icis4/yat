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
// Copyright � 2003-2004 HSR Hochschule f�r Technik Rapperswil.
// Copyright � 2003-2010 Matthias Kl�y.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Collections.Generic;
using System.Text;

namespace MKY.System.Xml
{
	/// <summary>
	/// Interface for XML contents that have alternate XML elements, e.g. when an XML element
	/// has changed its name.
	/// </summary>
	public interface IAlternateXmlElementProvider
	{
		/// <summary>
		/// Alternate XML elements. Applies to any kind of XML nodes.
		/// </summary>
		AlternateXmlElement[] AlternateXmlElements { get; }
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
