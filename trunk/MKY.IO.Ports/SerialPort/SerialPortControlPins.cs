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
using System.Xml.Serialization;

namespace MKY.IO.Ports
{
	/// <summary>
	/// Serial port control pins.
	/// </summary>
	[Serializable]
	public struct SerialPortControlPins
	{
		/// <summary>
		/// Request To Send.
		/// </summary>
		[XmlElement("Rts")]
		public bool Rts;

		/// <summary>
		/// Clear To Send.
		/// </summary>
		[XmlElement("Cts")]
		public bool Cts;

		/// <summary>
		/// Data Terminal Ready.
		/// </summary>
		[XmlElement("Dts")]
		public bool Dtr;

		/// <summary>
		/// Data Set Ready.
		/// </summary>
		[XmlElement("Dsr")]
		public bool Dsr;

		/// <summary>
		/// Carrier Detect.
		/// </summary>
		[XmlElement("Cd")]
		public bool Cd;
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
