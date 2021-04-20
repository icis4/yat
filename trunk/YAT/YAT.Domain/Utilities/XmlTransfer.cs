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
// YAT Version 2.4.1
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

#region Using
//==================================================================================================
// Using
//==================================================================================================

using System;
using System.Diagnostics.CodeAnalysis;
using System.Xml.Serialization;

#endregion

namespace YAT.Domain.Utilities
{
	/// <summary>
	/// A line's XML object model for import/export transfers.
	/// </summary>
	[Serializable]
	public abstract class XmlTransferLine
	{
		private DateTime  timeStamp;
		private string    device;
		private Direction direction;

		/// <summary></summary>
		protected XmlTransferLine()
		{
		}

		/// <summary></summary>
		protected XmlTransferLine(DateTime timeStamp, string device, Direction direction)
		{
			this.timeStamp = timeStamp;
			this.device    = device;
			this.direction = direction;
		}

		/// <summary></summary>
		[XmlAttribute("TimeStamp")]
		public virtual DateTime TimeStamp
		{
			get { return (this.timeStamp); }
			set { this.timeStamp = value;  }
		}

		/// <summary></summary>
		[XmlAttribute("Device")]
		public virtual string Device
		{
			get { return (this.device); }
			set { this.device = value;  }
		}

		/// <summary></summary>
		[XmlAttribute("Direction")]
		public virtual Direction Direction
		{
			get { return (this.direction); }
			set { this.direction = value;  }
		}
	}

	/// <summary>
	/// A line's XML object model for raw import/export transfers.
	/// </summary>
	[Serializable]
	public class XmlTransferRawLine : XmlTransferLine
	{
		private byte[] content;

		/// <summary></summary>
		public XmlTransferRawLine()
		{
		}

		/// <summary></summary>
		public XmlTransferRawLine(DateTime timeStamp, string device, Direction direction, byte[] content)
			: base(timeStamp, device, direction)
		{
			this.content = content;
		}

		/// <remarks>Data byte array is converted to Base64 encoded string.</remarks>
		[XmlAttribute("DataAsBase64")]
		public virtual string DataAsBase64
		{
			get { return (Convert.ToBase64String(this.content));  }
			set { this.content = Convert.FromBase64String(value); }
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays", Justification = "Guidelines for Collections: Do use byte arrays instead of collections of bytes.")]
		[XmlIgnore]
		public virtual byte[] Content
		{
			get { return (this.content); }
			set { this.content = value;  }
		}
	}

	/// <summary>
	/// A line's XML object model for text import/export transfers.
	/// </summary>
	[Serializable]
	public class XmlTransferTextLine : XmlTransferLine
	{
		private string text;
		private int    length;

		/// <summary></summary>
		public XmlTransferTextLine()
		{
		}

		/// <summary></summary>
		public XmlTransferTextLine(DateTime timeStamp, string device, Direction direction, string text, int length)
			: base(timeStamp, device, direction)
		{
			this.text   = text;
			this.length = length;
		}

		/// <summary></summary>
		[XmlAttribute("Text")]
		public virtual string Text
		{
			get { return (this.text); }
			set { this.text = value;  }
		}

		/// <summary></summary>
		[XmlAttribute("Length")]
		public virtual int Length
		{
			get { return (this.length); }
			set { this.length = value;  }
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
