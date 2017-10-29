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

		#region Object Members
		//==========================================================================================
		// Object Members
		//==========================================================================================

		/// <summary></summary>
		public override string ToString(string indent)
		{
			using (var sw = new StringWriter(CultureInfo.InvariantCulture))
			{
				foreach (byte b in Data)
					sw.Write(Convert.ToChar(b));

				return (indent + sw.ToString());
			}
		}

		/// <summary></summary>
		public override string ToDetailedString(string indent)
		{
			using (var sw = new StringWriter(CultureInfo.InvariantCulture))
			{
				bool begin = true;
				foreach (byte b in Data)
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
	/// Defines a text data item that shall be sent by the terminal.
	/// </summary>
	[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Parsable", Justification = "'Parsable' is a correct English term.")]
	public class ParsableDataSendItem : DataSendItem
	{
		/// <summary></summary>
		public string Data { get; }

		/// <summary></summary>
		public Radix DefaultRadix { get; }

		/// <summary></summary>
		public bool IsLine { get; }

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Default parameters result in cleaner code and clearly indicate the default behavior.")]
		public ParsableDataSendItem(string data, Radix defaultRadix = Parser.Parser.DefaultRadixDefault, bool isLine = false)
		{
			Data         = data;
			DefaultRadix = defaultRadix;
			IsLine       = isLine;
		}

		#region Object Members
		//==========================================================================================
		// Object Members
		//==========================================================================================

		/// <summary></summary>
		public override string ToString(string indent)
		{
			return (indent + Data);
		}

		/// <summary></summary>
		public override string ToDetailedString(string indent)
		{
			return (indent + "> Data         : " + Data         + Environment.NewLine +
					indent + "> DefaultRadix : " + DefaultRadix + Environment.NewLine +
					indent + "> IsLine       : " + IsLine       + Environment.NewLine);
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
		[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Default parameters result in cleaner code and clearly indicate the default behavior.")]
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
		public virtual string ToString(string indent)
		{
			return (indent + FilePath);
		}

		/// <summary></summary>
		public virtual string ToDetailedString(string indent)
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
