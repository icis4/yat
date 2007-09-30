using System;
using System.Collections.Generic;
using System.Text;

namespace YAT.Domain.IO
{
	/// <summary>
	/// Generic IO provider that is used for serial port and socket communication.
	/// </summary>
	public interface IIOProvider : IDisposable
	{
		//------------------------------------------------------------------------------------------
		// Events
		//------------------------------------------------------------------------------------------

		/// <summary>
		/// Fired after the IO provider's state has changed, e.g. started or stopped.
		/// </summary>
		event EventHandler IOChanged;

		/// <summary>
		/// Fired after the IO provider's control state has changed.
		/// </summary>
		/// <remarks>
		/// For serial ports, this event indicates a change of the serial control pins.
		/// </remarks>
		event EventHandler IOControlChanged;

		/// <summary>
		/// Fired after an IO error has occured.
		/// </summary>
		event EventHandler<IOErrorEventArgs> IOError;

		/// <summary>
		/// Fired after the IO provider has received data.
		/// </summary>
		event EventHandler DataReceived;

		/// <summary>
		/// Fired after the IO provider has sent data.
		/// </summary>
		event EventHandler DataSent;

		//------------------------------------------------------------------------------------------
		// Properties
		//------------------------------------------------------------------------------------------

		/// <summary>
		/// Gets a value indicating whether the IO provider has been started.
		/// </summary>
		/// <remarks>
		/// For serial ports, this property indicates that the port is open.
		/// </remarks>
		/// <returns>
		/// true if the IO provider has been started; otherwise, false.
		/// </returns>
		bool HasStarted { get; }

		/// <summary>
		/// Gets a value indicating whether the underlying IO instance
		/// is connected to a remote resource.
		/// </summary>
		/// <remarks>
		/// For serial ports, this property indicates that the port is open.
		/// </remarks>
		/// <returns>
		/// true if the underlying IO instance is connected to a
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
		/// Gets the underlying IO instance.
		/// </summary>
		/// <returns>
		/// The IO instance.
		/// </returns>
		object UnderlyingIOInstance { get; }

		//------------------------------------------------------------------------------------------
		// Methods
		//------------------------------------------------------------------------------------------

		/// <summary>
		/// Starts the IO provider.
		/// </summary>
		/// <remarks>
		/// For serial ports, the port is opened.
		/// </remarks>
		void Start();

		/// <summary>
		/// Stops the IO provider.
		/// </summary>
		/// <remarks>
		/// For serial ports, the port is closed.
		/// </remarks>
		void Stop();

		/// <summary>
		/// Receives data from a the underlying IO instance into a receive buffer.
		/// </summary>
		/// <param name="buffer">
		/// An array of type System.Byte that is the storage location for the received data.
		/// </param>
		/// <returns>The number of bytes received.</returns>
		int Receive(out byte[] buffer);

		/// <summary>
		/// Sends data to a the underlying IO instance.
		/// </summary>
		/// <param name="buffer">
		/// An array of type System.Byte that contains the data to be sent.
		/// </param>
		void Send(byte[] buffer);
	}
}
