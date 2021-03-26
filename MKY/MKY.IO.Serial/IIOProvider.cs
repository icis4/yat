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
// MKY Version 1.0.29
// ------------------------------------------------------------------------------------------------
// See release notes for product version details.
// See SVN change log for file revision details.
// Author(s): Matthias Klaey
// ------------------------------------------------------------------------------------------------
// Copyright © 2007-2021 Matthias Kläy.
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

using MKY.Contracts;

#endregion

namespace MKY.IO.Serial
{
	/// <summary>
	/// Generic I/O interface that is usable for any kind of serial communication.
	/// </summary>
	/// <remarks>
	/// While this interface is generic, it is instantiated by accompanying classes. Details on the
	/// behavior of these classes are contained in the remarks of the interface for convenience.
	/// </remarks>
	public interface IIOProvider : IDisposable, IDisposableEx
	{
		#region Events
		//==========================================================================================
		// Events
		//==========================================================================================

		/// <summary>
		/// Event raised after the I/O provider's state has changed, e.g. started or stopped.
		/// </summary>
		event EventHandler<EventArgs<DateTime>> IOChanged;

		/// <summary>
		/// Event raised after the I/O provider's control state has changed.
		/// </summary>
		/// <remarks><list type="bullet">
		/// <item><term>Serial COM Ports:</term><description>
		/// This event indicates a change of the serial control pins.
		/// </description></item>
		/// </list></remarks>
		event EventHandler<EventArgs<DateTime>> IOControlChanged;

		/// <summary>
		/// Event raised after an I/O warning is issued.
		/// </summary>
		event EventHandler<IOWarningEventArgs> IOWarning;

		/// <summary>
		/// Event raised after an I/O error has occurred.
		/// </summary>
		event EventHandler<IOErrorEventArgs> IOError;

		/// <summary>
		/// Event raised after the I/O provider has received data.
		/// </summary>
		/// <remarks>
		/// Opposed to the interface of <see cref="System.IO.Ports.SerialPort"/>, i.e. a method
		/// must be called to send and receive data, and the corresponding events do not contain
		/// any data, this event is implemented with data. There are several reasons for this:
		///  > Events with data are easier to use.
		///  > Events with data ensure that multiple recipients, i.e. event sinks, can use it.
		///  > Events with data can implemented in a way to prevent race conditions on handling.
		///
		/// Receive related code is located before send related code since I/O is a common term
		/// where I comes before O.
		/// </remarks>
		[CallingContract(IsNeverMainThread = true, IsAlwaysSequential = true)]
		event EventHandler<DataReceivedEventArgs> DataReceived;

		/// <summary>
		/// Event raised after the I/O provider has sent data.
		/// </summary>
		/// <remarks>
		/// Send related code is located after receive related code since I/O is a common term
		/// where I comes before O.
		/// </remarks>
		[CallingContract(IsNeverMainThread = true, IsAlwaysSequential = true)]
		event EventHandler<DataSentEventArgs> DataSent;

		#endregion

		#region Properties
		//==========================================================================================
		// Properties
		//==========================================================================================

		/// <summary>
		/// Gets a value indicating whether the I/O provider has been stopped.
		/// </summary>
		/// <returns>
		/// <c>true</c> if the I/O provider has been stopped; otherwise, <c>false</c>.
		/// </returns>
		bool IsStopped { get; }

		/// <summary>
		/// Gets a value indicating whether the I/O provider has been started.
		/// </summary>
		/// <returns>
		/// <c>true</c> if the I/O provider has been started; otherwise, <c>false</c>.
		/// </returns>
		bool IsStarted { get; }

		/// <summary>
		/// Gets a value indicating whether the I/O provider is started and the underlying I/O
		/// instance is open and/or connected to a remote resource.
		/// </summary>
		/// <remarks><list type="bullet">
		/// <item><term>Serial COM Ports:</term><description>
		/// This property indicates that the port is open and no break state is detected.
		/// The latter may not be supported by the I/O instance or driver.
		/// </description></item>
		/// <item><term>UDP/IP Sockets:</term><description>
		/// This property indicates that the socket is open.
		/// </description></item>
		/// <item><term>USB Ser/HID Devices:</term><description>
		/// This property indicates that the device is physically connected.
		/// </description></item>
		/// </list></remarks>
		/// <returns>
		/// <c>true</c> if the I/O provider is started and the underlying I/O instance is open and/or
		/// connected to a remote resource as of the most recent operation; otherwise, <c>false</c>.
		/// </returns>
		bool IsConnected { get; }

		/// <summary>
		/// Gets a value indicating whether the I/O provider is started and the underlying I/O
		/// instance is open.
		/// </summary>
		/// <remarks><list type="bullet">
		/// <item><term>TCP/IP Sockets:</term><description>
		/// This property indicates that the underlying I/O instance is connected to a remote resource.
		/// </description></item>
		/// </list></remarks>
		/// <returns>
		/// <c>true</c> if the I/O provider is started and the underlying I/O instance is open as of
		/// the most recent operation; otherwise, <c>false</c>.
		/// </returns>
		bool IsOpen { get; }

		/// <summary>
		/// Gets a value indicating whether the I/O provider is started and the underlying I/O
		/// instance is transmissive, i.e. ready to send and receive data.
		/// </summary>
		/// <remarks>
		/// This property has been added since the meaning of <see cref="IsConnected"/> and
		/// <see cref="IsOpen"/> differ depending on the underlying I/O instance.
		/// </remarks>
		/// <remarks>
		/// Additional 'IsReadyToSend' and 'IsReadyToReceive' could further refine the interface.
		/// </remarks>
		/// <returns>
		/// <c>true</c> if the I/O provider is started and the underlying I/O instance is
		/// transmissive, i.e. ready to send and receive data as of the most recent operation;
		/// otherwise, <c>false</c>.
		/// </returns>
		bool IsTransmissive { get; }

		/// <summary>
		/// Gets a value indicating whether the I/O provider and/or the underlying I/O instance
		/// is currently sending, about to start sending, or has pending data yet waiting to be
		/// sent (e.g. if flow control is active).
		/// </summary>
		/// <remarks><para>
		/// Named "IsSending" rather than e.g. "HasDataForSending" as it sounds more natural.
		/// </para><para>
		/// The capabilities of the underlying I/O instance differ, this property may or may not
		/// take the state of the underlying I/O instance into account.
		/// </para><para>
		/// Same with the the state of the underlying operating system driver and hardware. In most
		/// cases, their state cannot be retrieved from within a .NET implementation by common means.
		/// </para><list type="bullet">
		/// <item><term>Serial COM Ports:</term><description>
		/// The underlying <see cref="System.IO.Ports.SerialPort.BytesToWrite"/> is taken into account.
		/// </description></item>
		/// </list></remarks>
		/// <returns>
		/// <c>true</c> if the I/O provider and/or the underlying I/O instance is sending data;
		/// otherwise, <c>false</c>.
		/// </returns>
		bool IsSending { get; }

		// Opposed to 'IsSending', where the amount of data expected for sending is known by the
		// caller, there is no information on the availability and amout of data for receiving,
		// nor any way to determine whether data may be coming in soon. Thus, an 'IsReceiving'
		// indication makes little sense. Instead, the 'DataEventArgs.TimeStamp' value of the last
		// "DataReceived" event may be used to determine for how long the I/O instance has not been
		// receiving data anymore, and can combine this time stamp with a time-out.

		/// <summary>
		/// Gets the underlying I/O instance.
		/// </summary>
		/// <remarks>
		/// Two methods to lock/unlock the I/O instance could be added as the underlying logic could
		/// change or even dispose of the instance while accessing it.
		/// </remarks>
		/// <returns>
		/// The I/O instance.
		/// </returns>
		object UnderlyingIOInstance { get; }

		#endregion

		#region Methods
		//==========================================================================================
		// Methods
		//==========================================================================================

		/// <summary>
		/// Starts the I/O provider.
		/// </summary>
		/// <remarks><list type="bullet">
		/// <item><term>Serial COM Ports:</term><description>
		/// The port gets opened.
		/// </description></item>
		/// <item><term>TCP/IP Sockets:</term><description>
		/// The socket gets created and starts connecting/listening.
		/// </description></item>
		/// <item><term>UDP/IP Sockets:</term><description>
		/// The socket gets opened.
		/// </description></item>
		/// <item><term>USB Ser/HID Devices:</term><description>
		/// The device gets created.
		/// </description></item>
		/// </list></remarks>
		[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "'Ser/HID' just happens to contain 'Ser'...")]
		bool Start();

		/// <summary>
		/// Stops the I/O provider.
		/// </summary>
		/// <remarks><list type="bullet">
		/// <item><term>Serial COM Ports:</term><description>
		/// The port gets closed.
		/// </description></item>
		/// <item><term>TCP/IP Sockets:</term><description>
		/// The socket gets closed.
		/// </description></item>
		/// <item><term>UDP/IP Sockets:</term><description>
		/// The socket gets closed.
		/// </description></item>
		/// <item><term>USB Ser/HID Devices:</term><description>
		/// The device gets closed.
		/// </description></item>
		/// </list></remarks>
		[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "'Ser/HID' just happens to contain 'Ser'...")]
		[SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "Stop", Justification = "'Stop' is a common term to start/stop something.")]
		void Stop();

		/// <summary>
		/// Sends data using the underlying I/O instance.
		/// </summary>
		/// <remarks>
		/// If the underlying buffer has space, this method will immediately return; otherwise
		/// this method will be blocking until there is space, or the I/O instance is stopped
		/// or gets disconnected/closed.
		/// </remarks>
		/// <param name="data">
		/// An array of type <see cref="byte"/> that contains the data to be sent.
		/// </param>
		/// <returns>
		/// <c>true</c> if data has successfully been sent; otherwise, <c>false</c>.
		/// </returns>
		bool Send(byte[] data);

		/// <summary>
		/// Clears the send buffer of the I/O provider and the underlying I/O instance.
		/// </summary>
		/// <remarks>
		/// The buffer of the underlying I/O instance can only be cleared if this functionality
		/// is provided by the underlying I/O instance.
		/// </remarks>
		/// <returns>
		/// The number of bytes cleared.
		/// </returns>
		int ClearSendBuffer();

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
