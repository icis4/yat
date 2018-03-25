﻿//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at https://sourceforge.net/projects/y-a-terminal/.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Revision$
// $Date$
// $Author$
// ------------------------------------------------------------------------------------------------
// YAT Version 2.0.0
// ------------------------------------------------------------------------------------------------
// See release notes for product version details.
// See SVN change log for file revision details.
// Author(s): Matthias Klaey
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2018 Matthias Kläy.
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
		private string    port;
		private Direction direction;

		/// <summary></summary>
		protected XmlTransferLine()
		{
		}

		/// <summary></summary>
		protected XmlTransferLine(DateTime timeStamp, string port, Direction direction)
		{
			this.timeStamp = timeStamp;
			this.port      = port;
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
		[XmlAttribute("Port")]
		public virtual string Port
		{
			get { return (this.port); }
			set { this.port = value;  }
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
		public XmlTransferRawLine(DateTime timeStamp, string port, Direction direction, byte[] content)
			: base(timeStamp, port, direction)
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
	/// A line's XML object model for neat import/export transfers.
	/// </summary>
	[Serializable]
	public class XmlTransferNeatLine : XmlTransferLine
	{
		private string text;
		private string errorText;
		private int    length;

		/// <summary></summary>
		public XmlTransferNeatLine()
		{
		}

		/// <summary></summary>
		public XmlTransferNeatLine(DateTime timeStamp, string port, Direction direction, string text, string errorText, int length)
			: base(timeStamp, port, direction)
		{
			this.text      = text;
			this.errorText = errorText;
			this.length    = length;
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
