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
// YAT 2.0 Gamma 3 Development Version 1.99.53
// ------------------------------------------------------------------------------------------------
// See release notes for product version details.
// See SVN change log for file revision details.
// Author(s): Matthias Klaey
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2017 Matthias Kläy.
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
#if (DEBUG)

		/// <remarks>
		/// Note that it is not possible to mark a finalizer with [Conditional("DEBUG")].
		/// </remarks>
		[SuppressMessage("Microsoft.Performance", "CA1821:RemoveEmptyFinalizers", Justification = "See remarks.")]
		~SendItem()
		{
			MKY.Diagnostics.DebugFinalization.DebugNotifyFinalizerAndCheckWhetherOverdue(this);
		}

#endif // DEBUG

		#region Object Members
		//==========================================================================================
		// Object Members
		//==========================================================================================

		/// <summary>
		/// Converts the value of this instance to its equivalent string representation.
		/// </summary>
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

		#endregion
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

		#region Object Members
		//==========================================================================================
		// Object Members
		//==========================================================================================

		/// <summary></summary>
		public override string ToString(string indent)
		{
			using (StringWriter sw = new StringWriter(CultureInfo.InvariantCulture))
			{
				foreach (byte b in this.data)
					sw.Write(Convert.ToChar(b));

				return (indent + sw.ToString());
			}
		}

		/// <summary></summary>
		public override string ToDetailedString(string indent)
		{
			using (StringWriter sw = new StringWriter(CultureInfo.InvariantCulture))
			{
				bool begin = true;
				foreach (byte b in this.data)
				{
					if (!begin)
						sw.Write(" ");

					begin = false;
					sw.Write(b.ToString("X2", CultureInfo.InvariantCulture) + "h");
				}

				return (indent + "> Data: " + sw + Environment.NewLine);
			}
		}

		#endregion
	}

	/// <summary>
	/// Defines a text item that shall be sent by the terminal.
	/// </summary>
	[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Parsable", Justification = "'Parsable' is a correct English term.")]
	public class ParsableSendItem : SendItem
	{
		private string data;
		private Radix defaultRadix;
		private bool isLine;

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Default parameters result in cleaner code and clearly indicate the default behavior.")]
		public ParsableSendItem(string data, Radix defaultRadix = Parser.Parser.DefaultRadixDefault, bool isLine = false)
		{
			this.data = data;
			this.defaultRadix = defaultRadix;
			this.isLine = isLine;
		}

		/// <summary></summary>
		public virtual string Data
		{
			get { return (this.data); }
		}

		/// <summary></summary>
		public virtual Radix DefaultRadix
		{
			get { return (this.defaultRadix); }
		}

		/// <summary></summary>
		public virtual bool IsLine
		{
			get { return (this.isLine); }
		}

		#region Object Members
		//==========================================================================================
		// Object Members
		//==========================================================================================

		/// <summary></summary>
		public override string ToString(string indent)
		{
			return (indent + this.data);
		}

		/// <summary></summary>
		public override string ToDetailedString(string indent)
		{
			return (indent + "> Data         : " + this.data         + Environment.NewLine +
					indent + "> DefaultRadix : " + this.defaultRadix + Environment.NewLine +
					indent + "> IsLine       : " + this.isLine       + Environment.NewLine);
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
