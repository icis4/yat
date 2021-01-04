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
// YAT Version 2.2.0 Development
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
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text;

using MKY.Contracts;

#endregion

// The YAT.Domain namespace contains all raw/neutral/binary/text terminal infrastructure. This code
// is intentionally placed into the YAT.Domain namespace even though the file is located in
// YAT.Domain\RawTerminal for better separation of the implementation files.
namespace YAT.Domain
{
	/// <summary>
	/// Defines an element received from or sent to a serial interface. In addition to the raw byte
	/// content itself, it also contains interface and time information.
	/// </summary>
	[ImmutableObject(true)]
	[ImmutableContract(true)]
	public class RawChunk
	{
		#region Properties
		//==========================================================================================
		// Properties
		//==========================================================================================

		/// <remarks>
		/// "Guidelines for Collections: Do use byte arrays instead of collections of bytes."
		/// is overruled in order to be able to implement this class as immutable.
		/// </remarks>
		public ReadOnlyCollection<byte> Content { get; }

		/// <summary></summary>
		public DateTime TimeStamp { get; }

		/// <summary></summary>
		public string Device { get; }

		/// <summary></summary>
		public IODirection Direction { get; }

		#endregion

		#region Object Lifetime
		//==========================================================================================
		// Object Lifetime
		//==========================================================================================

		/// <summary></summary>
		public RawChunk(byte[] content, string device, IODirection direction)
			: this(content, DateTime.Now, device, direction)
		{
		}

		/// <summary></summary>
		public RawChunk(byte[] content, DateTime timeStamp, string device, IODirection direction)
		{
			Content   = new ReadOnlyCollection<byte>((byte[])content.Clone());
			TimeStamp = timeStamp;
			Device    = device;
			Direction = direction;
		}

	#if (DEBUG)

		/// <remarks>
		/// Note that it is not possible to mark a finalizer with [Conditional("DEBUG")].
		/// </remarks>
		[SuppressMessage("Microsoft.Performance", "CA1821:RemoveEmptyFinalizers", Justification = "See remarks.")]
		~RawChunk()
		{
			MKY.Diagnostics.DebugFinalization.DebugNotifyFinalizerAndCheckWhetherOverdue(this);
		}

	#endif // DEBUG

		#endregion

		#region Methods
		//==========================================================================================
		// Methods
		//==========================================================================================

		/// <summary></summary>
		public virtual string ContentToString()
		{
			return (Utilities.ByteHelper.FormatHexString(Content, Settings.DisplaySettings.ShowRadixDefault));
		}

		#endregion

		#region Object Members
		//==========================================================================================
		// Object Members
		//==========================================================================================

		/// <summary>
		/// Converts the value of this instance to its equivalent string representation.
		/// </summary>
		public override string ToString()
		{
			var sb = new StringBuilder();

			sb.Append(TimeStamp.ToString("HH:mm:ss.fff", DateTimeFormatInfo.CurrentInfo));
			sb.Append(" ");
			sb.Append(Device);
			sb.Append(" ");
			sb.Append(Direction);
			sb.Append(" ");
			sb.Append(ContentToString()); // Content last to get better aligned output.

			return (sb.ToString());
		}

		/// <summary>
		/// Converts the value of this instance to its equivalent string representation.
		/// </summary>
		/// <remarks>
		/// Extended <see cref="ToString()"/> method which can be used for trace/debug.
		/// </remarks>
		[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Default parameters may result in cleaner code and clearly indicate the default behavior.")]
		public virtual string ToExtendedDiagnosticsString(string indent = "")
		{
			var sb = new StringBuilder();

			sb.AppendLine(indent + "> TimeStamp: " + TimeStamp.ToString("HH:mm:ss.fff", DateTimeFormatInfo.CurrentInfo));
			sb.AppendLine(indent + "> Device:    " + Device);
			sb.AppendLine(indent + "> Direction: " + Direction);
			sb.AppendLine(indent + "> Content:   " + ContentToString()); // Content same as ToString().

			return (sb.ToString());
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
