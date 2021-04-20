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
// MKY Version 1.0.30
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

using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text;

namespace MKY.IO.Ports
{
	/// <summary>
	/// ISerialPortDotNet is the interface of <see cref="System.IO.Ports.SerialPort"/>.
	/// </summary>
	public interface ISerialPortDotNet : IComponent
	{
		#region Mapped SerialPort Events
		//==========================================================================================
		// Mapped SerialPort Events
		//------------------------------------------------------------------------------------------
		// Must be mapped because System.IO.Ports.Serial<EventType>EventArgs do
		// not provide any constructor and can therefore not be created from an
		// adapter like MKY.IO.Ports.SerialPortEx
		//==========================================================================================

		/// <summary>
		/// Represents the method that will handle the data received event
		/// of a <see cref="System.IO.Ports.SerialPort"/> object.
		/// </summary>
		event SerialDataReceivedEventHandler DataReceived;

		/// <summary>
		/// Represents the method that handles the error event
		/// of a <see cref="System.IO.Ports.SerialPort"/> object.
		/// </summary>
		event SerialErrorReceivedEventHandler ErrorReceived;

		/// <summary>
		/// Represents the method that will handle the serial pin changed event
		/// of a <see cref="System.IO.Ports.SerialPort"/> object.
		/// </summary>
		/// <remarks>
		/// Attention: No event is fired if the RTS or DTR line is changed.
		/// </remarks>
		event SerialPinChangedEventHandler PinChanged;

		#endregion

		#region 1:1 code compatible interface
		//==========================================================================================
		// 1:1 code compatible interface
		//==========================================================================================

		/// <summary>
		/// Gets or sets the port for communications, including but not limited to
		/// all available COM ports.
		/// </summary>
		/// <value>
		/// The communications port. The default is COM1.
		/// </value>
		/// <exception cref="System.ArgumentNullException">
		/// The <see cref="System.IO.Ports.SerialPort.PortName"/> property was set to <c>null</c>.
		/// </exception>
		/// <exception cref="System.ArgumentException">
		/// The <see cref="System.IO.Ports.SerialPort.PortName"/> property was set to a value with
		/// a length of zero. -or- The <see cref="System.IO.Ports.SerialPort.PortName"/> property
		/// was set to a value that starts with "\\". -or- The port name was not valid.
		/// </exception>
		/// <exception cref="System.InvalidOperationException">
		/// The specified port is open.
		/// </exception>
		string PortName { get; set; }

		/// <summary>
		/// Gets the underlying <see cref="Stream"/> object for
		/// a <see cref="System.IO.Ports.SerialPort"/> object.
		/// </summary>
		/// <exception cref="InvalidOperationException">
		/// The stream is closed. This can occur because the <see cref="Open"/> method
		/// has not been called or the <see cref="Close"/> method has been called.
		/// </exception>
		Stream BaseStream { get; }

		/// <summary>
		/// Gets or sets the serial baud rate.
		/// </summary>
		/// <value>
		/// The baud rate.
		/// </value>
		/// <exception cref="System.ArgumentOutOfRangeException">
		/// The baud rate specified is less than or equal to zero, or is greater than
		/// the maximum allowable baud rate for the device.
		/// </exception>
		/// <exception cref="System.IO.IOException">
		/// The port is in an invalid state. - or - An attempt to set the state of
		/// the underlying port failed. For example, the parameters passed from this
		/// <see cref="System.IO.Ports.SerialPort"/> object were invalid.
		/// </exception>
		int BaudRate { get; set; }

		/// <summary>
		/// Gets or sets the standard length of data bits per byte.
		/// </summary>
		/// <value>
		/// The data bits length.
		/// </value>
		/// <exception cref="System.IO.IOException">
		/// The port is in an invalid state.  - or -An attempt to set the state of
		/// the underlying port failed. For example, the parameters passed from this
		/// <see cref="System.IO.Ports.SerialPort"/> object were invalid.
		/// </exception>
		/// <exception cref="System.ArgumentOutOfRangeException">
		/// The data bits value is less than 5 or more than 8.
		/// </exception>
		int DataBits { get; set; }

		/// <summary>
		/// Gets or sets the parity-checking protocol.
		/// </summary>
		/// <value>
		/// One of the <see cref="System.IO.Ports.Parity"/> values that represents
		/// the parity-checking protocol. The default is None.
		/// </value>
		/// <exception cref="System.IO.IOException">
		/// The port is in an invalid state. - or -An attempt to set the state of
		/// the underlying port failed. For example, the parameters passed from this
		/// <see cref="System.IO.Ports.SerialPort"/> object were invalid.
		/// </exception>
		/// <exception cref="System.ArgumentOutOfRangeException">
		/// The <see cref="System.IO.Ports.SerialPort.Parity"/> value passed is not
		/// a valid value in the <see cref="System.IO.Ports.Parity"/> enumeration.
		/// </exception>
		System.IO.Ports.Parity Parity { get; set; }

		/// <summary>
		/// Gets or sets the standard number of stop bits per byte.
		/// </summary>
		/// <value>
		/// One of the <see cref="System.IO.Ports.StopBits"/> values.
		/// </value>
		/// <exception cref="System.ArgumentOutOfRangeException">
		/// The <see cref="System.IO.Ports.SerialPort.StopBits"/> value is not one
		/// of the values from the <see cref="System.IO.Ports.StopBits"/> enumeration.
		/// </exception>
		/// <exception cref="System.IO.IOException">
		/// The port is in an invalid state. - or - An attempt to set the state of
		/// the underlying port failed. For example, the parameters passed from this
		/// <see cref="System.IO.Ports.SerialPort"/> object were invalid.
		/// </exception>
		System.IO.Ports.StopBits StopBits { get; set; }

		/// <summary>
		/// Gets or sets the handshaking protocol for serial port transmission of data.
		/// </summary>
		/// <value>
		/// One of the <see cref="System.IO.Ports.Handshake"/> values. The default is None.
		/// </value>
		/// <exception cref="System.InvalidOperationException">
		/// The stream is closed. This can occur because the
		/// <see cref="System.IO.Ports.SerialPort.Open()"/> method has not been
		/// called or the <see cref="System.IO.Ports.SerialPort.Close()"/> method
		/// has been called.
		/// </exception>
		/// <exception cref="System.IO.IOException">
		/// The port is in an invalid state. - or - An attempt to set the state of
		/// the underlying port failed. For example, the parameters passed from this
		/// <see cref="System.IO.Ports.SerialPort"/> object were invalid.
		/// </exception>
		/// <exception cref="System.ArgumentOutOfRangeException">
		/// The value passed is not a valid value in the
		/// <see cref="System.IO.Ports.Handshake"/> enumeration.
		/// </exception>
		System.IO.Ports.Handshake Handshake { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether the Request To Send (RTS) signal
		/// is enabled during serial communication.
		/// </summary>
		/// <value>
		/// <c>true</c> to enable Request to Transmit (RTS); otherwise, <c>false</c>.
		/// The default is <c>false</c>.
		/// </value>
		/// <remarks>
		/// Attention: No <see cref="PinChanged"/> event is fired.
		/// </remarks>
		/// <exception cref="System.InvalidOperationException">
		/// The value of the <see cref="System.IO.Ports.SerialPort.RtsEnable"/>
		/// property was set or retrieved while the
		/// <see cref="System.IO.Ports.SerialPort.Handshake"/> property is set to the
		/// <see cref="System.IO.Ports.Handshake.RequestToSend"/> value or the
		/// <see cref="System.IO.Ports.Handshake.RequestToSendXOnXOff"/> value.
		/// </exception>
		/// <exception cref="System.IO.IOException">
		/// The port is in an invalid state. - or - An attempt to set the state of
		/// the underlying port failed. For example, the parameters passed from this
		/// <see cref="System.IO.Ports.SerialPort"/> object were invalid.
		/// </exception>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Rts", Justification = "'RTS' is a common term for serial ports.")]
		bool RtsEnable { get; set; }

		/// <summary>
		/// Gets the state of the Clear-to-Send line.
		/// </summary>
		/// <value>
		/// <c>true</c> if the Clear-to-Send line is detected; otherwise, <c>false</c>.
		/// </value>
		/// <exception cref="System.InvalidOperationException">
		/// The stream is closed. This can occur because the
		/// <see cref="System.IO.Ports.SerialPort.Open()"/> method has not been called
		/// or the <see cref="System.IO.Ports.SerialPort.Close()"/> method has been
		/// called.
		/// </exception>
		/// <exception cref="System.IO.IOException">
		/// The port is in an invalid state.  - or - An attempt to set the state of the
		/// underlying port failed. For example, the parameters passed from this
		/// <see cref="System.IO.Ports.SerialPort"/> object were invalid.
		/// </exception>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Cts", Justification = "'CTS' is a common term for serial ports.")]
		bool CtsHolding { get; }

		/// <summary>
		/// Gets or sets a value that enables the Data Terminal Ready (DTR) signal
		/// during serial communication.
		/// </summary>
		/// <value>
		/// <c>true</c> to enable Data Terminal Ready (DTR); otherwise, <c>false</c>.
		/// The default is <c>false</c>.
		/// </value>
		/// <remarks>
		/// Attention: No <see cref="PinChanged"/> event is fired.
		/// </remarks>
		/// <exception cref="System.IO.IOException">
		/// The port is in an invalid state. - or - An attempt to set the state of
		/// the underlying port failed. For example, the parameters passed from this
		/// <see cref="System.IO.Ports.SerialPort"/> object were invalid.
		/// </exception>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Dtr", Justification = "'DTR' is a common term for serial ports.")]
		bool DtrEnable { get; set; }

		/// <summary>
		/// Gets the state of the Data Set Ready (DSR) signal.
		/// </summary>
		/// <value>
		/// <c>true</c> if a Data Set Ready signal has been sent to the port;
		/// otherwise, <c>false</c>.
		/// </value>
		/// <exception cref="System.InvalidOperationException">
		/// The stream is closed. This can occur because the
		/// <see cref="System.IO.Ports.SerialPort.Open()"/> method has not been called
		/// or the <see cref="System.IO.Ports.SerialPort.Close()"/> method has been
		/// called.
		/// </exception>
		/// <exception cref="System.IO.IOException">
		/// The port is in an invalid state.  - or - An attempt to set the state of the
		/// underlying port failed. For example, the parameters passed from this
		/// <see cref="System.IO.Ports.SerialPort"/> object were invalid.
		/// </exception>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Dsr", Justification = "'DSR' is a common term for serial ports.")]
		bool DsrHolding { get; }

		/// <summary>
		/// Gets the state of the Carrier Detect line for the port.
		/// </summary>
		/// <value>
		/// <c>true</c> if the carrier is detected; otherwise, <c>false</c>.
		/// </value>
		/// <exception cref="System.InvalidOperationException">
		/// The stream is closed. This can occur because the
		/// <see cref="System.IO.Ports.SerialPort.Open()"/> method has not been called
		/// or the <see cref="System.IO.Ports.SerialPort.Close()"/> method has been
		/// called.
		/// </exception>
		/// <exception cref="System.IO.IOException">
		/// The port is in an invalid state.  - or - An attempt to set the state of the
		/// underlying port failed. For example, the parameters passed from this
		/// <see cref="System.IO.Ports.SerialPort"/> object were invalid.
		/// </exception>
		bool CDHolding { get; }

		/// <summary>
		/// Gets or sets the break signal state.
		/// </summary>
		/// <value>
		/// <c>true</c> if the port is in a break state; otherwise, <c>false</c>.
		/// </value>
		/// <exception cref="System.IO.IOException">
		/// The port is in an invalid state. - or - An attempt to set the state of the
		/// underlying port failed. For example, the parameters passed from this
		/// <see cref="System.IO.Ports.SerialPort"/> object were invalid.
		/// </exception>
		/// <exception cref="System.InvalidOperationException">
		/// The stream is closed. This can occur because the
		/// <see cref="System.IO.Ports.SerialPort.Open()"/> method has not been called
		/// or the <see cref="System.IO.Ports.SerialPort.Close()"/> method has been
		/// called.
		/// </exception>
		bool BreakState { get; set; }

		/// <summary>
		/// Gets or sets the value used to interpret the end of a call to the
		/// <see cref="System.IO.Ports.SerialPort.ReadLine()"/> and
		/// <see cref="System.IO.Ports.SerialPort.WriteLine(System.String)"/>
		/// methods.
		/// </summary>
		/// <value>
		/// A value that represents the end of a line. The default is a line feed,
		/// <see cref="System.Environment.NewLine"/>.
		/// </value>
		/// <exception cref="System.ArgumentNullException">
		/// The <see cref="System.IO.Ports.SerialPort.IsOpen"/> value passed is <c>null</c>.
		/// </exception>
		/// <exception cref="System.ArgumentException">
		/// The <see cref="System.IO.Ports.SerialPort.IsOpen"/> value passed is an
		/// empty string ("").
		/// </exception>
		string NewLine { get; set; }

		/// <summary>
		/// Gets or sets the byte encoding for pre- and post-transmission conversion
		/// of text.
		/// </summary>
		/// <value>
		/// An <see cref="System.Text.Encoding"/> object. The default is
		/// <see cref="System.Text.ASCIIEncoding"/>.
		/// </value>
		/// <exception cref="System.ArgumentNullException">
		/// The <see cref="System.IO.Ports.SerialPort.Encoding"/> property was set to <c>null</c>.
		/// </exception>
		/// <exception cref="System.ArgumentException">
		/// The <see cref="System.IO.Ports.SerialPort.Encoding"/> property was set to
		/// an encoding that is not <see cref="System.Text.ASCIIEncoding"/>,
		/// <see cref="System.Text.UTF8Encoding"/>, <see cref="System.Text.UTF32Encoding"/>,
		/// <see cref="System.Text.UnicodeEncoding"/>, one of the Windows single byte
		/// encodings, or one of the Windows double byte encodings.
		/// </exception>
		Encoding Encoding { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether null bytes are ignored when
		/// transmitted between the port and the receive buffer.
		/// </summary>
		/// <value>
		/// <c>true</c> if null bytes are ignored; otherwise, <c>false</c>.
		/// The default is <c>false</c>.
		/// </value>
		/// <exception cref="System.InvalidOperationException">
		/// The stream is closed. This can occur because the
		/// <see cref="System.IO.Ports.SerialPort.Open()"/> method has not been
		/// called or the <see cref="System.IO.Ports.SerialPort.Close()"/> method
		/// has been called.
		/// </exception>
		/// <exception cref="System.IO.IOException">
		/// The port is in an invalid state. - or - An attempt to set the state of
		/// the underlying port failed. For example, the parameters passed from this
		/// <see cref="System.IO.Ports.SerialPort"/> object were invalid.
		/// </exception>
		bool DiscardNull { get; set; }

		/// <summary>
		/// Gets or sets the byte that replaces invalid bytes in a data stream when
		/// a parity error occurs.
		/// </summary>
		/// <value>
		/// A byte that replaces invalid bytes.
		/// </value>
		/// <exception cref="System.IO.IOException">
		/// The port is in an invalid state. - or - An attempt to set the state of
		/// the underlying port failed. For example, the parameters passed from this
		/// <see cref="System.IO.Ports.SerialPort"/> object were invalid.
		/// </exception>
		byte ParityReplace { get; set; }

		/// <summary>
		/// Gets or sets the size of the <see cref="System.IO.Ports.SerialPort"/>
		/// input buffer.
		/// </summary>
		/// <value>
		/// The buffer size. The default value is 4096.
		/// </value>
		/// <exception cref="System.ArgumentOutOfRangeException">
		/// The <see cref="System.IO.Ports.SerialPort.ReadBufferSize"/> value set is
		/// less than or equal to zero.
		/// </exception>
		/// <exception cref="System.IO.IOException">
		/// The <see cref="System.IO.Ports.SerialPort.ReadBufferSize"/> property was
		/// set to an odd integer value.
		/// </exception>
		/// <exception cref="System.InvalidOperationException">
		/// The <see cref="System.IO.Ports.SerialPort.ReadBufferSize"/> property was
		/// set while the stream was open.
		/// </exception>
		int ReadBufferSize { get; set; }

		/// <summary>
		/// Gets or sets the number of milliseconds before a time-out occurs when a
		/// read operation does not finish.
		/// </summary>
		/// <value>
		/// The number of milliseconds before a time-out occurs when a read operation
		/// does not finish.
		/// </value>
		/// <exception cref="System.ArgumentOutOfRangeException">
		/// The read time-out value is less than zero and not equal to
		/// <see cref="System.IO.Ports.SerialPort.InfiniteTimeout"/>.
		/// </exception>
		/// <exception cref="System.IO.IOException">
		/// The port is in an invalid state. - or - An attempt to set the state of
		/// the underlying port failed. For example, the parameters passed from this
		/// <see cref="System.IO.Ports.SerialPort"/> object were invalid.
		/// </exception>
		int ReadTimeout { get; set; }

		/// <summary>
		/// Gets or sets the number of bytes in the internal input buffer before a
		/// <see cref="System.IO.Ports.SerialPort.DataReceived"/> event occurs.
		/// </summary>
		/// <value>
		/// The number of bytes in the internal input buffer before a
		/// <see cref="System.IO.Ports.SerialPort.DataReceived"/> event is fired.
		/// The default is 1.
		/// </value>
		/// <exception cref="System.ArgumentOutOfRangeException">
		/// The <see cref="System.IO.Ports.SerialPort.ReceivedBytesThreshold"/>
		/// value is less than or equal to zero.
		/// </exception>
		int ReceivedBytesThreshold { get; set; }

		/// <summary>
		/// Gets or sets the size of the serial port output buffer.
		/// </summary>
		/// <value>
		/// The size of the output buffer. The default is 2048.
		/// </value>
		/// <exception cref="System.ArgumentOutOfRangeException">
		/// The <see cref="System.IO.Ports.SerialPort.WriteBufferSize"/> value is
		/// less than or equal to zero.
		/// </exception>
		/// <exception cref="System.InvalidOperationException">
		/// The <see cref="System.IO.Ports.SerialPort.WriteBufferSize"/> property
		/// was set while the stream was open.
		/// </exception>
		/// <exception cref="System.IO.IOException">
		/// The <see cref="System.IO.Ports.SerialPort.WriteBufferSize"/> property
		/// was set to an odd integer value.
		/// </exception>
		int WriteBufferSize { get; set; }

		/// <summary>
		/// Gets or sets the number of milliseconds before a time-out occurs when a
		/// write operation does not finish.
		/// </summary>
		/// <value>
		/// The number of milliseconds before a time-out occurs. The default is
		/// <see cref="System.IO.Ports.SerialPort.InfiniteTimeout"/>.
		/// </value>
		/// <exception cref="System.ArgumentOutOfRangeException">
		/// The <see cref="System.IO.Ports.SerialPort.WriteTimeout"/> value is less than
		/// zero and not equal to <see cref="System.IO.Ports.SerialPort.InfiniteTimeout"/>.
		/// </exception>
		/// <exception cref="System.IO.IOException">
		/// The port is in an invalid state. - or - An attempt to set the state of
		/// the underlying port failed. For example, the parameters passed from this
		/// <see cref="System.IO.Ports.SerialPort"/> object were invalid.
		/// </exception>
		int WriteTimeout { get; set; }

		/// <summary>
		/// Gets a value indicating the open or closed status of the
		/// <see cref="System.IO.Ports.SerialPort"/> object.
		/// </summary>
		/// <value>
		/// <c>true</c> if the serial port is open; otherwise, <c>false</c>.
		/// The default is <c>false</c>.
		/// </value>
		bool IsOpen { get; }

		/// <summary>
		/// Gets the number of bytes of data in the receive buffer.
		/// </summary>
		/// <value>
		/// The number of bytes of data in the receive buffer.
		/// </value>
		/// <exception cref="System.InvalidOperationException">
		/// The stream is closed. This can occur because the
		/// <see cref="System.IO.Ports.SerialPort.Open()"/> method has not been called
		/// or the <see cref="System.IO.Ports.SerialPort.Close()"/> method has been
		/// called.
		/// </exception>
		/// <exception cref="System.IO.IOException">
		/// The port is in an invalid state.
		/// </exception>
		int BytesToRead { get; }

		/// <summary>
		/// Gets the number of bytes of data in the send buffer.
		/// </summary>
		/// <value>
		/// The number of bytes of data in the send buffer.
		/// </value>
		/// <exception cref="System.InvalidOperationException">
		/// The stream is closed. This can occur because the
		/// <see cref="System.IO.Ports.SerialPort.Open()"/> method has not been called
		/// or the <see cref="System.IO.Ports.SerialPort.Close()"/> method has been
		/// called.
		/// </exception>
		/// <exception cref="System.IO.IOException">
		/// The port is in an invalid state.
		/// </exception>
		int BytesToWrite { get; }

		/// <summary>
		/// Opens a new serial port connection.
		/// </summary>
		/// <exception cref="System.UnauthorizedAccessException">
		/// Access is denied to the port.
		/// </exception>
		/// <exception cref="System.ArgumentException">
		/// The port name does not begin with "COM". - or - The file type of the port
		/// is not supported.
		/// </exception>
		/// <exception cref="System.IO.IOException">
		/// The port is in an invalid state. - or - An attempt to set the state of
		/// the underlying port failed. For example, the parameters passed from this
		/// <see cref="System.IO.Ports.SerialPort"/> object were invalid.
		/// </exception>
		/// <exception cref="System.ArgumentOutOfRangeException">
		/// One or more of the properties for this instance are invalid. For example,
		/// the <see cref="System.IO.Ports.SerialPort.Parity"/>,
		/// <see cref="System.IO.Ports.SerialPort.DataBits"/>, or
		/// <see cref="System.IO.Ports.SerialPort.Handshake"/> properties are not valid
		/// values; the <see cref="System.IO.Ports.SerialPort.BaudRate"/> is less than or
		/// equal to zero; the <see cref="System.IO.Ports.SerialPort.ReadTimeout"/> or
		/// <see cref="System.IO.Ports.SerialPort.WriteTimeout"/> property is less than
		/// zero and is not <see cref="System.IO.Ports.SerialPort.InfiniteTimeout"/>.
		/// </exception>
		/// <exception cref="System.InvalidOperationException">
		/// The specified port is open.
		/// </exception>
		void Open();

		/// <summary>
		/// Closes the port connection, sets the
		/// <see cref="System.IO.Ports.SerialPort.IsOpen"/> property to false,
		/// and disposes of the internal <see cref="System.IO.Stream"/> object.
		/// </summary>
		/// <exception cref="System.InvalidOperationException">
		/// The specified port is not open.
		/// </exception>
		[Obsolete("This standard variant of Close() doesn't know the cirumstances. Use ISerialPortEx.CloseNormally() or ISerialPortEx.CloseAfterException() instead.")]
		void Close();

		/// <summary>
		/// Reads a number of bytes from the
		/// <see cref="System.IO.Ports.SerialPort"/> input buffer and writes
		/// those bytes into a byte array at the specified offset.
		/// </summary>
		/// <param name="buffer">The byte array to write the input to.</param>
		/// <param name="offset">The offset in the buffer array to begin writing.</param>
		/// <param name="count">The number of bytes to read.</param>
		/// <returns>The number of bytes read.</returns>
		/// <exception cref="System.TimeoutException">
		/// No bytes were available to read.
		/// </exception>
		/// <exception cref="System.ArgumentException">
		/// offset plus count is greater than the length of the buffer.
		/// </exception>
		/// <exception cref="System.ArgumentOutOfRangeException">
		/// The offset or count parameters are outside a valid region of the
		/// buffer being passed. Either offset or count is less than zero.
		/// </exception>
		/// <exception cref="System.ArgumentNullException">
		/// The buffer passed is <c>null</c>.
		/// </exception>
		/// <exception cref="System.InvalidOperationException">
		/// The specified port is not open.
		/// </exception>
		int Read(byte[] buffer, int offset, int count);

		/// <summary>
		/// Reads a number of bytes from the
		/// <see cref="System.IO.Ports.SerialPort"/> input buffer and writes
		/// those bytes into a byte array at a given offset.
		/// </summary>
		/// <param name="buffer">The character array to write the input to.</param>
		/// <param name="offset">The offset in the buffer array to begin writing.</param>
		/// <param name="count">The number of bytes to read.</param>
		/// <returns>The number of bytes read.</returns>
		/// <exception cref="System.TimeoutException">
		/// No bytes were available to read.
		/// </exception>
		/// <exception cref="System.ArgumentException">
		/// offset plus count is greater than the length of the buffer. - or -
		/// count is 1 and there is a surrogate character in the buffer.
		/// </exception>
		/// <exception cref="System.ArgumentOutOfRangeException">
		/// The offset or count parameters are outside a valid region of the buffer
		/// being passed. Either offset or count is less than zero.
		/// </exception>
		/// <exception cref="System.ArgumentNullException">
		/// The buffer passed is <c>null</c>.
		/// </exception>
		/// <exception cref="System.InvalidOperationException">
		/// The specified port is not open.
		/// </exception>
		int Read(char[] buffer, int offset, int count);

		/// <summary>
		/// Synchronously reads one byte from the
		/// <see cref="System.IO.Ports.SerialPort"/> input buffer.
		/// </summary>
		/// <returns>The byte that was read.</returns>
		/// <exception cref="System.TimeoutException">
		/// The operation did not complete before the time-out period ended.
		/// - or - No byte was read.
		/// </exception>
		/// <exception cref="System.InvalidOperationException">
		/// The specified port is not open.
		/// </exception>
		int ReadByte();

		/// <summary>
		/// Synchronously reads one character from the
		/// <see cref="System.IO.Ports.SerialPort"/> input buffer.
		/// </summary>
		/// <returns>The character that was read.</returns>
		/// <exception cref="System.TimeoutException">
		/// The operation did not complete before the time-out period ended.
		/// - or - No character was available in the allotted time-out period.
		/// </exception>
		/// <exception cref="System.InvalidOperationException">
		/// The specified port is not open.
		/// </exception>
		int ReadChar();

		/// <summary>
		/// Reads all immediately available bytes, based on the encoding,
		/// in both the stream and the input buffer of the
		/// <see cref="System.IO.Ports.SerialPort"/> object.
		/// </summary>
		/// <returns>
		/// The contents of the stream and the input buffer of the
		/// <see cref="System.IO.Ports.SerialPort"/> object.
		/// </returns>
		/// <exception cref="System.InvalidOperationException">
		/// The specified port is not open.
		/// </exception>
		string ReadExisting();

		/// <summary>
		/// Reads up to the <see cref="System.IO.Ports.SerialPort.NewLine"/>
		/// value in the input buffer.
		/// </summary>
		/// <returns>
		/// The contents of the input buffer up to the
		/// <see cref="System.IO.Ports.SerialPort.NewLine"/> value.
		/// </returns>
		/// <exception cref="System.TimeoutException">
		/// The operation did not complete before the time-out period ended.
		/// - or - No bytes were read.
		/// </exception>
		/// <exception cref="System.InvalidOperationException">
		/// The specified port is not open.
		/// </exception>
		string ReadLine();

		/// <summary>
		/// Reads a string up to the specified value in the input buffer.
		/// </summary>
		/// <param name="value">A value that indicates where the read operation stops.</param>
		/// <returns>The contents of the input buffer up to the specified value.</returns>
		/// <exception cref="System.ArgumentNullException">
		/// The value parameter is <c>null</c>.
		/// </exception>
		/// <exception cref="System.TimeoutException">
		/// The operation did not complete before the time-out period ended.
		/// </exception>
		/// <exception cref="System.ArgumentException">
		/// The length of the value parameter is 0.
		/// </exception>
		/// <exception cref="System.InvalidOperationException">
		/// The specified port is not open.
		/// </exception>
		string ReadTo(string value);

		/// <summary>
		/// Writes the parameter string to the output.
		/// </summary>
		/// <param name="text">The string for output.</param>
		/// <exception cref="System.TimeoutException">
		/// The operation did not complete before the time-out period ended.
		/// </exception>
		/// <exception cref="System.ArgumentNullException">
		/// str is <c>null</c>.
		/// </exception>
		/// <exception cref="System.InvalidOperationException">
		/// The specified port is not open.
		/// </exception>
		void Write(string text);

		/// <summary>
		/// Writes a specified number of bytes to an output buffer at the
		/// specified offset.
		/// </summary>
		/// <param name="buffer">The byte array to write the output to.</param>
		/// <param name="offset">The offset in the buffer array to begin writing.</param>
		/// <param name="count">The number of bytes to write.</param>
		/// <exception cref="System.ArgumentException">
		/// offset plus count is greater than the length of the buffer.
		/// </exception>
		/// <exception cref="System.TimeoutException">
		/// The operation did not complete before the time-out period ended.
		/// </exception>
		/// <exception cref="System.ArgumentOutOfRangeException">
		/// The offset or count parameters are outside a valid region of the
		/// buffer being passed. Either offset or count is less than zero.
		/// </exception>
		/// <exception cref="System.ArgumentNullException">
		/// The buffer passed is <c>null</c>.
		/// </exception>
		/// <exception cref="System.InvalidOperationException">
		/// The specified port is not open.
		/// </exception>
		void Write(byte[] buffer, int offset, int count);

		/// <summary>
		/// Writes a specified number of characters to an output buffer at the
		/// specified offset.
		/// </summary>
		/// <param name="buffer">The character array to write the output to.</param>
		/// <param name="offset">The offset in the buffer array to begin writing.</param>
		/// <param name="count">The number of characters to write.</param>
		/// <exception cref="System.ArgumentException">
		/// offset plus count is greater than the length of the buffer.
		/// </exception>
		/// <exception cref="System.TimeoutException">
		/// The operation did not complete before the time-out period ended.
		/// </exception>
		/// <exception cref="System.ArgumentOutOfRangeException">
		/// The offset or count parameters are outside a valid region of the
		/// buffer being passed. Either offset or count is less than zero.
		/// </exception>
		/// <exception cref="System.ArgumentNullException">
		/// The buffer passed is <c>null</c>.
		/// </exception>
		/// <exception cref="System.InvalidOperationException">
		/// The specified port is not open.
		/// </exception>
		void Write(char[] buffer, int offset, int count);

		/// <summary>
		/// Writes the specified string and the
		/// <see cref="System.IO.Ports.SerialPort.NewLine"/> value to the output buffer.
		/// </summary>
		/// <param name="text">The string to write to the output buffer.</param>
		/// <exception cref="System.TimeoutException">
		/// The <see cref="System.IO.Ports.SerialPort.WriteLine(System.String)"/>
		/// method could not write to the stream.
		/// </exception>
		/// <exception cref="System.ArgumentNullException">
		/// The str parameter is <c>null</c>.
		/// </exception>
		/// <exception cref="System.InvalidOperationException">
		/// The specified port is not open.
		/// </exception>
		void WriteLine(string text);

		/// <summary>
		/// Discards data from the serial driver's receive buffer.
		/// </summary>
		/// <exception cref="System.IO.IOException">
		/// The port is in an invalid state. - or - An attempt to set the state of
		/// the underlying port failed. For example, the parameters passed from this
		/// <see cref="System.IO.Ports.SerialPort"/> object were invalid.
		/// </exception>
		/// <exception cref="System.InvalidOperationException">
		/// The stream is closed. This can occur because the
		/// <see cref="System.IO.Ports.SerialPort.Open()"/> method has not been
		/// called or the <see cref="System.IO.Ports.SerialPort.Close()"/> method
		/// has been called.
		/// </exception>
		void DiscardInBuffer();

		/// <summary>
		/// Discards data from the serial driver's transmit buffer.
		/// </summary>
		/// <exception cref="System.InvalidOperationException">
		/// The stream is closed. This can occur because the
		/// <see cref="System.IO.Ports.SerialPort.Open()"/> method has not been
		/// called or the <see cref="System.IO.Ports.SerialPort.Close()"/> method
		/// has been called.
		/// </exception>
		/// <exception cref="System.IO.IOException">
		/// The port is in an invalid state. - or - An attempt to set the state of
		/// the underlying port failed. For example, the parameters passed from this
		/// <see cref="System.IO.Ports.SerialPort"/> object were invalid.
		/// </exception>
		void DiscardOutBuffer();

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
