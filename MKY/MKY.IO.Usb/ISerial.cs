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
// MKY Version 1.0.28 Development
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

using System;
using System.Diagnostics.CodeAnalysis;

namespace MKY.IO.Usb
{
	/// <summary>
	/// Interface for serial communication, e.g. Ser/CDC (i.e. CDC/ACM) or Ser/HID.
	/// </summary>
	/// <remarks>
	/// This interface is defined similar to <see cref="System.IO.Ports.SerialPort"/>,
	/// i.e. a  method must be called to send and receive data, and the corresponding
	/// events do not contain any data.
	/// </remarks>
	[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "'Ser/HID' just happens to contain 'Ser'...")]
	public interface ISerial
	{
		#region Events
		//==========================================================================================
		// Events
		//==========================================================================================

		/// <summary>
		/// Event raised after port successfully opened.
		/// </summary>
		event EventHandler Opened;

		/// <summary>
		/// Event raised after port successfully closed.
		/// </summary>
		event EventHandler Closed;

		/// <summary>
		/// Event raised after data has been received from the device.
		/// </summary>
		event EventHandler DataReceived;

		/// <summary>
		/// Event raised after data has completely be sent to the device.
		/// </summary>
		event EventHandler<DataEventArgs> DataSent;

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
		[SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "Stop", Justification = "'Stop' is a common term to start/stop something.")]
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
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "0#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
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
