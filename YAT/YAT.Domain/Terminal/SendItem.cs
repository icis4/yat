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
// YAT 2.0 Gamma 2 Development Version 1.99.35
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// See release notes for product version details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2016 Matthias Kläy.
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
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;

using MKY;

#endregion

// The YAT.Domain namespace contains all raw/neutral/binary/text terminal infrastructure. This code
// is intentionally placed into the YAT.Domain namespace even though the file is located in the
// YAT.Domain\Terminal for better separation of the implementation files.
namespace YAT.Domain
{
	/// <summary>
	/// Defines an item that shall be sent by the terminal.
	/// </summary>
	public abstract class SendItem
	{
		/// <summary></summary>
		public override string ToString()
		{
			return (ToString(""));
		}

		/// <summary></summary>
		public abstract string ToString(string indent);

		/// <summary></summary>
		public virtual string ToDetailedString()
		{
			return (ToDetailedString(""));
		}

		/// <summary></summary>
		public abstract string ToDetailedString(string indent);
	}

	/// <summary>
	/// Defines a binary item that shall be sent by the terminal.
	/// </summary>
	public class RawSendItem : SendItem
	{
		private byte[] data;

		/// <summary></summary>
		public RawSendItem(byte[] data)
		{
			this.data = data;
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays", Justification = "Source is an array, sink is an array, this class transports the array from source to sink, there's no purpose to use a ReadOnlyCollection here.")]
		public virtual byte[] Data
		{
			get { return (this.data); }
		}

		/// <summary></summary>
		public override string ToString(string indent)
		{
			StringWriter to = new StringWriter(CultureInfo.InvariantCulture);
			foreach (byte b in this.data)
				to.Write(Convert.ToChar(b));

			return (indent + to.ToString());
		}

		/// <summary></summary>
		public override string ToDetailedString(string indent)
		{
			bool begin = true;
			StringWriter data = new StringWriter(CultureInfo.InvariantCulture);
			foreach (byte b in this.data)
			{
				if (!begin)
					data.Write(" ");

				begin = false;
				data.Write(b.ToString("X2", CultureInfo.InvariantCulture) + "h");
			}

			return (indent + "> Data: " + data + Environment.NewLine);
		}
	}

	/// <summary>
	/// Defines a text item that shall be sent by the terminal.
	/// </summary>
	[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Parsable", Justification = "'Parsable' is a correct English term.")]
	public class ParsableSendItem : SendItem
	{
		private string data;
		private bool isLine;

		/// <summary></summary>
		public ParsableSendItem(string data, bool isLine)
		{
			this.data = data;
			this.isLine = isLine;
		}

		/// <summary></summary>
		public virtual string Data
		{
			get { return (this.data); }
		}

		/// <summary></summary>
		public virtual bool IsLine
		{
			get { return (this.isLine); }
		}

		/// <summary></summary>
		public override string ToString(string indent)
		{
			return (indent + this.data);
		}

		/// <summary></summary>
		public override string ToDetailedString(string indent)
		{
			return (indent + "> Data: " + this.data + Environment.NewLine +
					indent + "> IsLine: " + this.isLine + Environment.NewLine);
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
