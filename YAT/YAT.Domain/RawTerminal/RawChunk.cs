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
// YAT Version 2.0.1 Development
// ------------------------------------------------------------------------------------------------
// See release notes for product version details.
// See SVN change log for file revision details.
// Author(s): Matthias Klaey
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2019 Matthias Kläy.
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

using MKY;
using MKY.Contracts;

#endregion

// The YAT.Domain namespace contains all raw/neutral/binary/text terminal infrastructure. This code
// is intentionally placed into the YAT.Domain namespace even though the file is located in the
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
		#region Constants
		//==========================================================================================
		// Constants
		//==========================================================================================

		private const string Undefined = "<Undefined>";

		#endregion

		#region Properties
		//==========================================================================================
		// Properties
		//==========================================================================================

		/// <remarks>
		/// "Guidelines for Collections: Do use byte arrays instead of collections of bytes."
		/// is overruled in order to be able to implement this class of immutable objects.
		/// </remarks>
		public ReadOnlyCollection<byte> Content { get; }

		/// <summary></summary>
		public DateTime TimeStamp { get; }

		/// <summary></summary>
		public string PortStamp { get; }

		/// <summary></summary>
		public IODirection Direction { get; }

		#endregion

		#region Object Lifetime
		//==========================================================================================
		// Object Lifetime
		//==========================================================================================

		/// <summary></summary>
		public RawChunk(byte[] content, IODirection direction)
			: this(content, Undefined, direction)
		{
		}

		/// <summary></summary>
		public RawChunk(byte[] content, string portStamp, IODirection direction)
			: this(content, DateTime.Now, portStamp, direction)
		{
		}

		/// <summary></summary>
		public RawChunk(byte[] content, DateTime timeStamp, string portStamp, IODirection direction)
		{
			Content   = new ReadOnlyCollection<byte>((byte[])content.Clone());
			TimeStamp = timeStamp;
			PortStamp = portStamp;
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
		protected string ContentAsPrintableString
		{
			get { return (Utilities.ByteHelper.FormatHexString(Content)); }
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
			return (ContentAsPrintableString);
		}

		/// <summary>
		/// Converts the value of this instance to its equivalent string representation.
		/// </summary>
		/// <remarks>
		/// Extended <see cref="ToString()"/> method which can be used for trace/debug.
		/// </remarks>
		public virtual string ToDiagnosticsString()
		{
			return (ToDiagnosticsString(""));
		}

		/// <summary>
		/// Converts the value of this instance to its equivalent string representation.
		/// </summary>
		/// <remarks>
		/// Extended <see cref="ToString()"/> method which can be used for trace/debug.
		/// </remarks>
		public virtual string ToDiagnosticsString(string indent)
		{
			var sb = new StringBuilder();

			sb.AppendLine(indent + "> Content:   " + ContentAsPrintableString);
			sb.AppendLine(indent + "> TimeStamp: " + TimeStamp.ToLongTimeString() + "." + StringEx.Left(TimeStamp.Millisecond.ToString("D3", CultureInfo.CurrentCulture), 2));
			sb.AppendLine(indent + "> PortStamp: " + PortStamp);
			sb.AppendLine(indent + "> Direction: " + Direction);

			return (sb.ToString());
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================