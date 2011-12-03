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
// MKY Version 1.0.7
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

namespace MKY.IO.Usb
{
	/// <summary>
	/// Interface for serial communication, e.g. Ser/CDC or Ser/HID.
	/// </summary>
	public interface ISerial
	{
		#region Events
		//==========================================================================================
		// Events
		//==========================================================================================

		/// <summary>
		/// Fired after port successfully opened.
		/// </summary>
		event EventHandler Opened;

		/// <summary>
		/// Fired after port successfully closed.
		/// </summary>
		event EventHandler Closed;

		/// <summary>
		/// Fired after data has been received from the device.
		/// </summary>
		event EventHandler DataReceived;

		/// <summary>
		/// Fired after data has completely be sent to the device.
		/// </summary>
		event EventHandler DataSent;

		#endregion

		#region Properties
		//==========================================================================================
		// Properties
		//==========================================================================================

		/// <summary>
		/// Indicates whether the device automatically tries to reconnect.
		/// </summary>
		/// <returns>
		/// <c>true</c> if the device automatically tries to reconnect; otherwise, <c>false</c>.
		/// </returns>
		bool AutoOpen { get; set; }

		/// <summary>
		/// Gets a value indicating whether the device has been started.
		/// </summary>
		/// <returns>
		/// <c>true</c> if the device has been started; otherwise, <c>false</c>.
		/// </returns>
		bool IsStarted { get; }

		/// <summary>
		/// Indicates whether the serial communication port to the device is open.
		/// </summary>
		/// <returns>
		/// <c>true</c> if the serial communication port is open; otherwise, <c>false</c>.
		/// </returns>
		bool IsOpen { get; }

		/// <summary>
		/// Gets the amount of data received from the device that is available to read.
		/// </summary>
		/// <returns>
		/// The number of bytes of data received from the device.
		/// </returns>
		int BytesAvailable { get; }

		#endregion

		#region Methods
		//==========================================================================================
		// Methods
		//==========================================================================================

		/// <summary>
		/// Starts the device.
		/// </summary>
		bool Start();

		/// <summary>
		/// Stops the device.
		/// </summary>
		[SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "Stop", Justification = "Stop is a common term to start/stop something.")]
		void Stop();

		/// <summary>
		/// Opens the serial communication port to the device.
		/// </summary>
		bool Open();

		/// <summary>
		/// Closes the serial communication port to the device.
		/// </summary>
		void Close();

		/// <summary>
		/// Receives data from the device into a receive buffer.
		/// </summary>
		/// <param name="data">
		/// An array of type System.Byte that is the storage location for the received data.
		/// </param>
		/// <returns>The number of bytes received.</returns>
		int Receive(out byte[] data);

		/// <summary>
		/// Sends data to the device.
		/// </summary>
		/// <param name="data">
		/// An array of type System.Byte that contains the data to be sent.
		/// </param>
		void Send(byte[] data);

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
