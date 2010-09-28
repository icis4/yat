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
// YAT is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace YAT.Settings
{
	/// <summary></summary>
	[Serializable]
	public struct SaveInfo
	{
		/// <summary></summary>
		[XmlElement("TimeStamp")]
		public DateTime TimeStamp;

		/// <summary></summary>
		[XmlElement("UserName")]
		public string UserName;

		/// <summary></summary>
		public SaveInfo(DateTime timeStamp, string userName)
		{
			TimeStamp = timeStamp;
			UserName = userName;
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
