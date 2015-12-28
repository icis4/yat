﻿//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at https://sourceforge.net/projects/y-a-terminal/.
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

#region Using
//==================================================================================================
// Using
//==================================================================================================

using System;
using System.Xml.Serialization;

using YAT.Domain;

#endregion

namespace YAT.Model.Utilities
{
	/// <summary>
	/// A line's XML object model for import/export transfers.
	/// </summary>
	[Serializable]
	public abstract class XmlTransferLine
	{
		private DateTime timeStamp;
		private Direction direction;
		private int length;

		/// <summary></summary>
		public XmlTransferLine()
		{
		}

		/// <summary></summary>
		public XmlTransferLine(DateTime timeStamp, Direction direction, int length)
		{
			this.timeStamp = timeStamp;
			this.direction = direction;
			this.length    = length;
		}

		/// <summary></summary>
		[XmlAttribute("TimeStamp")]
		public virtual DateTime TimeStamp
		{
			get { return (this.timeStamp); }
			set { this.timeStamp = value;  }
		}

		/// <summary></summary>
		[XmlAttribute("Direction")]
		public virtual Direction Direction
		{
			get { return (this.direction); }
			set { this.direction = value;  }
		}

		/// <summary></summary>
		[XmlAttribute("Length")]
		public virtual int Length
		{
			get { return (this.length); }
			set { this.length = value;  }
		}
	}

	/// <summary>
	/// A line's XML object model for raw import/export transfers.
	/// </summary>
	[Serializable]
	public class XmlTransferRawLine : XmlTransferLine
	{
		private byte[] data;

		/// <summary></summary>
		public XmlTransferRawLine()
		{
		}

		/// <summary></summary>
		public XmlTransferRawLine(DateTime timeStamp, Direction direction, int length, byte[] data)
			: base (timeStamp, direction, length)
		{
			this.data = data;
		}

		/// <remarks>Data byte array is converted to Base64 encoded string.</remarks>
		[XmlAttribute("Data")]
		public virtual string Data
		{
			get { return (Convert.ToBase64String(this.data));  }
			set { this.data = Convert.FromBase64String(value); }
		}
	}

	/// <summary>
	/// A line's XML object model for neat import/export transfers.
	/// </summary>
	[Serializable]
	public class XmlTransferNeatLine : XmlTransferLine
	{
		private string text;
		private string errorText;

		/// <summary></summary>
		public XmlTransferNeatLine()
		{
		}

		/// <summary></summary>
		public XmlTransferNeatLine(DateTime timeStamp, Direction direction, int length, string text, string errorText)
			: base(timeStamp, direction, length)
		{
			this.text = text;
			this.errorText = errorText;
		}

		/// <summary></summary>
		[XmlAttribute("Text")]
		public virtual string Text
		{
			get { return (this.text); }
			set { this.text = value;  }
		}

		/// <summary></summary>
		[XmlAttribute("ErrorText")]
		public virtual string ErrorText
		{
			get { return (this.errorText); }
			set { this.errorText = value;  }
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
