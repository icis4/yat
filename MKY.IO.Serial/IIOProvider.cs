using System;
using System.Collections.Generic;
using System.Text;

namespace MKY.IO.Serial
{
	/// <summary>
	/// Generic I/O provider that is used for serial port and socket communication.
	/// </summary>
	public interface IIOProvider : IDisposable
	{
		#region Events
		//==========================================================================================
		// Events
		//==========================================================================================

		/// <summary>
		/// Fired after the I/O provider's state has changed, e.g. started or stopped.
		/// </summary>
		event EventHandler IOChanged;

		/// <summary>
		/// Fired after the I/O provider's control state has changed.
		/// </summary>
		/// <remarks>
		/// For serial ports, this event indicates a change of the serial control pins.
		/// </remarks>
		event EventHandler IOControlChanged;

		/// <summary>
		/// Fired after an I/O error has occured.
		/// </summary>
		event EventHandler<IOErrorEventArgs> IOError;

		/// <summary>
		/// Fired after the I/O provider has received data.
		/// </summary>
		event EventHandler DataReceived;

		/// <summary>
		/// Fired after the I/O provider has sent data.
		/// </summary>
		event EventHandler DataSent;

		#endregion

		#region Properties
		//==========================================================================================
		// Properties
		//==========================================================================================

		/// <summary>
		/// Gets a value indicating whether the I/O provider has been started.
		/// </summary>
		/// <returns>
		/// true if the I/O provider has been started; otherwise, false.
		/// </returns>
		bool IsStarted { get; }

		/// <summary>
		/// Gets a value indicating whether the underlying I/O instance is open.
		/// </summary>
		/// <remarks>
		/// For TCP sockets, this property indicates that the the underlying I/O
		/// instance is connected to a remote resource.
		/// </remarks>
		/// <returns>
		/// true if the underlying I/O instance is open as of the most recent
		/// operation; otherwise, false.
		/// </returns>
		bool IsOpen { get; }

		/// <summary>
		/// Gets a value indicating whether the underlying I/O instance
		/// is open and/or connected to a remote resource.
		/// </summary>
		/// <remarks>
		/// For serial ports, this property indicates that the port is open
		/// and no break state is detected (if supported by the I/O instance).
		/// </remarks>
		/// <remarks>
		/// For UDP sockets, this property indicates that the socket is open.
		/// </remarks>
		/// <returns>
		/// true if the underlying I/O instance is open and/or connected to a
		/// remote resource as of the most recent operation; otherwise, false.
		/// </returns>
		bool IsConnected { get; }

		/// <summary>
		/// Gets the amount of data received from the remote resource that is available to read.
		/// </summary>
		/// <returns>
		/// The number of bytes of data received from the remote resource.
		/// </returns>
		int BytesAvailable { get; }

		/// <summary>
		/// Gets the underlying I/O instance.
		/// </summary>
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
		/// <remarks>
		/// For serial ports, the port is opened.
		/// </remarks>
		void Start();

		/// <summary>
		/// Stops the I/O provider.
		/// </summary>
		/// <remarks>
		/// For serial ports, the port is closed.
		/// </remarks>
		void Stop();

		/// <summary>
		/// Receives data from a the underlying I/O instance into a receive buffer.
		/// </summary>
		/// <param name="buffer">
		/// An array of type System.Byte that is the storage location for the received data.
		/// </param>
		/// <returns>The number of bytes received.</returns>
		int Receive(out byte[] buffer);

		/// <summary>
		/// Sends data to a the underlying I/O instance.
		/// </summary>
		/// <param name="buffer">
		/// An array of type System.Byte that contains the data to be sent.
		/// </param>
		void Send(byte[] buffer);

		#endregion
	}
}
