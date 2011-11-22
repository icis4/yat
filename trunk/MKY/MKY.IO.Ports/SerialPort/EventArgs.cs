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
// MKY Development Version 1.0.6
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// See release notes for product version details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2011 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace MKY.IO.Ports
{
	/// <summary></summary>
	[Serializable]
	public class SerialDataReceivedEventArgs : EventArgs
	{
		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = "Public fields are straight-forward for event args.")]
		public readonly System.IO.Ports.SerialData EventType;

		/// <summary></summary>
		public SerialDataReceivedEventArgs(System.IO.Ports.SerialData eventType)
		{
			EventType = eventType;
		}
	}

	/// <summary></summary>
	[Serializable]
	public class SerialErrorReceivedEventArgs : EventArgs
	{
		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = "Public fields are straight-forward for event args.")]
		public readonly System.IO.Ports.SerialError EventType;

		/// <summary></summary>
		public SerialErrorReceivedEventArgs(System.IO.Ports.SerialError eventType)
		{
			EventType = eventType;
		}
	}

	/// <summary></summary>
	[Serializable]
	public class SerialPinChangedEventArgs : EventArgs
	{
		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = "Public fields are straight-forward for event args.")]
		public readonly MKY.IO.Ports.SerialPinChange EventType;

		/// <summary></summary>
		public SerialPinChangedEventArgs(MKY.IO.Ports.SerialPinChange eventType)
		{
			EventType = eventType;
		}
	}

	/// <summary></summary>
	public delegate void SerialDataReceivedEventHandler(object sender, SerialDataReceivedEventArgs e);

	/// <summary></summary>
	public delegate void SerialErrorReceivedEventHandler(object sender, SerialErrorReceivedEventArgs e);

	/// <summary></summary>
	public delegate void SerialPinChangedEventHandler(object sender, SerialPinChangedEventArgs e);
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
