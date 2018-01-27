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
// YAT 2.0 Epsilon Version 1.99.90
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
using System.Globalization;
using System.IO;
using System.Text;

using MKY;

#endregion

// The YAT.Domain namespace contains all raw/neutral/binary/text terminal infrastructure. This code
// is intentionally placed into the YAT.Domain namespace even though the file is located in the
// YAT.Domain\Terminal for better separation of the implementation files.
namespace YAT.Domain
{
	/// <summary>
	/// Defines a data item that shall be sent by the terminal.
	/// </summary>
	public abstract class DataSendItem
	{
	#if (DEBUG)

		/// <remarks>
		/// Note that it is not possible to mark a finalizer with [Conditional("DEBUG")].
		/// </remarks>
		[SuppressMessage("Microsoft.Performance", "CA1821:RemoveEmptyFinalizers", Justification = "See remarks.")]
		~DataSendItem()
		{
			MKY.Diagnostics.DebugFinalization.DebugNotifyFinalizerAndCheckWhetherOverdue(this);
		}

	#endif // DEBUG

		#region Object Members
		//==========================================================================================
		// Object Members
		//==========================================================================================

		/// <summary></summary>
		public virtual string ToDiagnosticsString()
		{
			return (ToDiagnosticsString(""));
		}

		/// <summary></summary>
		public abstract string ToDiagnosticsString(string indent);

		#endregion
	}

	/// <summary>
	/// Defines a binary raw data item that shall be sent by the terminal.
	/// </summary>
	public class RawDataSendItem : DataSendItem
	{
		/// <summary></summary>
		[SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays", Justification = "Source is an array, sink is an array, this class transports the array from source to sink, there's no purpose to use a ReadOnlyCollection here.")]
		public byte[] Data { get; }

		/// <summary></summary>
		public RawDataSendItem(byte[] data)
		{
			Data = data;
		}

		/// <summary></summary>
		protected string DataAsPrintableString
		{
			get
			{
				var sb = new StringBuilder();

				bool isFirst = true;
				foreach (byte b in Data)
				{
					if (isFirst)
						isFirst = false;
					else
						sb.Append(" ");

					sb.Append(b.ToString("X2", CultureInfo.InvariantCulture) + "h");
				}

				return (sb.ToString());
			}
		}

		#region Object Members
		//==========================================================================================
		// Object Members
		//==========================================================================================

		/// <summary></summary>
		public override string ToString()
		{
			return (DataAsPrintableString);
		}

		/// <summary></summary>
		public override string ToDiagnosticsString(string indent)
		{
			return (indent + "> Data: " + DataAsPrintableString + Environment.NewLine);
		}

		#endregion
	}

	/// <summary>
	/// Defines a text data item that shall be sent by the terminal.
	/// </summary>
	[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Parsable", Justification = "'Parsable' is a correct English term.")]
	public class TextDataSendItem : DataSendItem
	{
		/// <summary></summary>
		public string Data { get; }

		/// <summary></summary>
		public Radix DefaultRadix { get; }

		/// <summary></summary>
		public Parser.Modes ParseMode { get; }

		/// <summary></summary>
		public bool IsLine { get; }

		/// <summary></summary>
		public TextDataSendItem(string data, Radix defaultRadix, Parser.Modes parseMode, bool isLine)
		{
			Data         = data;
			DefaultRadix = defaultRadix;
			ParseMode    = parseMode;
			IsLine       = isLine;
		}

		/// <summary></summary>
		protected string DataAsPrintableString
		{
			get { return (StringEx.ConvertToPrintableString(Data)); }
		}

		#region Object Members
		//==========================================================================================
		// Object Members
		//==========================================================================================

		/// <summary></summary>
		public override string ToString()
		{
			return (DataAsPrintableString);
		}

		/// <summary></summary>
		public override string ToDiagnosticsString(string indent)
		{
			return (indent + "> Data         : " + DataAsPrintableString + Environment.NewLine +
			        indent + "> DefaultRadix : " + DefaultRadix          + Environment.NewLine +
			        indent + "> ParseMode    : " + ParseMode             + Environment.NewLine +
			        indent + "> IsLine       : " + IsLine                + Environment.NewLine);
		}

		#endregion
	}

	/// <summary>
	/// Defines a file item that shall be sent by the terminal.
	/// </summary>
	public class FileSendItem
	{
		/// <summary></summary>
		public string FilePath { get; }

		/// <summary></summary>
		public Radix DefaultRadix { get; }

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Default parameters may result in cleaner code and clearly indicate the default behavior.")]
		public FileSendItem(string filePath, Radix defaultRadix = Parser.Parser.DefaultRadixDefault)
		{
			FilePath     = filePath;
			DefaultRadix = defaultRadix;
		}

		#region Object Members
		//==========================================================================================
		// Object Members
		//==========================================================================================

		/// <summary></summary>
		public override string ToString()
		{
			return (FilePath);
		}

		/// <summary></summary>
		public virtual string ToDiagnosticsString(string indent)
		{
			return (indent + "> FilePath     : " + FilePath     + Environment.NewLine +
			        indent + "> DefaultRadix : " + DefaultRadix + Environment.NewLine);
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
